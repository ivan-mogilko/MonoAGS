﻿using System;
using AGS.API;
using System.Collections.Generic;
using Autofac;
using System.Diagnostics;
using System.ComponentModel;

namespace AGS.Engine
{
	public class AGSRendererLoop : IRendererLoop
	{
        private readonly IGameState _gameState;
        private readonly IGame _game;
		private readonly Resolver _resolver;
		private readonly IAGSRoomTransitions _roomTransitions;
        private readonly IMatrixUpdater _matrixUpdater;
        private readonly IWindowInfo _window;
        private readonly IDisplayList _displayList;
        private readonly IInput _input;
        private readonly IGameSettings _noAspectRatioSettings;
        private readonly IAGSRenderPipeline _pipeline;
        private readonly DummyWindow _dummyWindow;
        private IGLUtils _glUtils;
        private IShader _lastShaderUsed;
		
        private IFrameBuffer _fromTransitionBuffer, _toTransitionBuffer;        

		public AGSRendererLoop (Resolver resolver, IGame game,
            IAGSRoomTransitions roomTransitions, IGLUtils glUtils, IWindowInfo window,
            IAGSRenderPipeline pipeline, IDisplayList displayList, 
            IInput input, IMatrixUpdater matrixUpdater)
		{
            _dummyWindow = new DummyWindow();
            _pipeline = pipeline;
            _input = input;
            _displayList = displayList;
            _glUtils = glUtils;
            _window = window;
			_resolver = resolver;
            _game = game;
			_gameState = game.State;
            _noAspectRatioSettings = new AGSGameSettings(game.Settings.Title, game.Settings.VirtualResolution, preserveAspectRatio: false);
			_roomTransitions = roomTransitions;
            _matrixUpdater = matrixUpdater;
			_roomTransitions.Transition = new RoomTransitionInstant ();
		}

		#region IRendererLoop implementation

        public bool Tick()
        {
            _glUtils.RefreshViewport(_game.Settings, _window, _gameState.Viewport, true);
            _glUtils.AdjustResolution(_game.Settings.VirtualResolution.Width, _game.Settings.VirtualResolution.Height);

			var transitionState = _roomTransitions.State;
            if (_gameState.Room == null) transitionState = RoomTransitionState.NotInTransition; //If there's no room, then room transition state is meaningless -> we'll interpret as not in transition, which will render the viewports, without a room (so GUIs will still be rendered).
			switch (transitionState)
			{
				case RoomTransitionState.NotInTransition:
					activateShader();
					renderAllViewports();
					break;
				case RoomTransitionState.BeforeLeavingRoom:
                    if (_roomTransitions.Transition == null)
                    {
                        _roomTransitions.State = RoomTransitionState.NotInTransition;
                        return false;
                    }
                    else if (_gameState.Cutscene.IsSkipping)
                    {
                        _roomTransitions.State = RoomTransitionState.PreparingTransition;
                        return false;
                    }
                    else if (!_roomTransitions.Transition.RenderBeforeLeavingRoom(
                        _displayList.GetDisplayList(_gameState.Viewport)))
					{
						if (_fromTransitionBuffer == null) _fromTransitionBuffer = renderToBuffer();
                        _roomTransitions.State = RoomTransitionState.PreparingTransition;
						return false;
					}
					break;
                case RoomTransitionState.PreparingNewRoomRendering:
				case RoomTransitionState.PreparingTransition:
					return false;
				case RoomTransitionState.InTransition:
                    if (_gameState.Cutscene.IsSkipping)
                    { 
                        _fromTransitionBuffer = null;
                        _toTransitionBuffer = null;
                        _roomTransitions.State = RoomTransitionState.AfterEnteringRoom;
                        return false;
                    }
					if (_toTransitionBuffer == null) _toTransitionBuffer = renderToBuffer();
                    _dummyWindow.GameSubWindow = new Rectangle(0, 0, (int)_window.AppWindowWidth, (int)_window.AppWindowHeight);
                    _glUtils.RefreshViewport(_noAspectRatioSettings, _dummyWindow, _gameState.Viewport, false);
                    if (!_roomTransitions.Transition.RenderTransition(_fromTransitionBuffer, _toTransitionBuffer))
					{
						_fromTransitionBuffer = null;
						_toTransitionBuffer = null;
						_roomTransitions.State = RoomTransitionState.AfterEnteringRoom;
						return false;
					}
					break;
				case RoomTransitionState.AfterEnteringRoom:
                    if (_gameState.Cutscene.IsSkipping || !_roomTransitions.Transition.RenderAfterEnteringRoom(
                        _displayList.GetDisplayList(_gameState.Viewport)))
					{
						_roomTransitions.SetOneTimeNextTransition(null);
						_roomTransitions.State = RoomTransitionState.NotInTransition;
						return false;
					}
					break;
				default:
					throw new NotSupportedException (_roomTransitions.State.ToString());
			}
			return true;
		}

		#endregion

        private IFrameBuffer renderToBuffer()
		{
            TypedParameter sizeParam = new TypedParameter(typeof(Size), new Size(
                (int)_window.AppWindowWidth, (int)_window.AppWindowHeight));
            IFrameBuffer frameBuffer = _resolver.Container.Resolve<IFrameBuffer>(sizeParam);
			frameBuffer.Begin();
			renderAllViewports();
			frameBuffer.End();
			return frameBuffer;
		}

        private void renderAllViewports()
		{
            _matrixUpdater.ClearCache();
            var instructionSet = _pipeline.InstructionSet;
            if (instructionSet == null) return;
            foreach (var (viewport, instructions) in instructionSet)
            {
                renderViewport(viewport, instructions);
            }
		}

        private void renderViewport(IViewport viewport, List<IRenderBatch> instructions)
        {
            _glUtils.RefreshViewport(_game.Settings, _window, viewport, true);

            foreach (var batch in instructions)
            {
                renderBatch(batch);
            }
        }

        private void renderBatch(IRenderBatch batch)
        {
            _glUtils.AdjustResolution(batch.Resolution.Width, batch.Resolution.Height);

            var shader = applyObjectShader(batch.Shader);

            foreach (var instruction in batch.Instructions)
            {
                instruction.Render();
                instruction.Release();
            }

            removeObjectShader(shader);
        }

		private static IShader applyObjectShader(IShader shader)
		{
			if (shader != null) shader = shader.Compile();
			shader?.Bind();
			return shader;
		}

		private void removeObjectShader(IShader shader)
		{
			if (shader == null) return;

			if (_lastShaderUsed != null) _lastShaderUsed.Bind();
			else shader.Unbind();
		}

		private void activateShader()
		{
			var shader = AGSGame.Shader;
			if (shader != null) shader = shader.Compile();
			if (shader == null)
			{
		        _lastShaderUsed?.Unbind();
				return;
			}
			_lastShaderUsed = shader;
			shader.Bind();
		}

        private class DummyWindow : IWindowInfo
        {
            public float AppWindowHeight => throw new NotImplementedException();
            public float AppWindowWidth => throw new NotImplementedException();
            public Rectangle GameSubWindow { get; set; }
            public Rectangle ScreenViewport { get; set; }
            #pragma warning disable CS0067
            public event PropertyChangedEventHandler PropertyChanged;
            #pragma warning restore CS0067
        }
	}
}