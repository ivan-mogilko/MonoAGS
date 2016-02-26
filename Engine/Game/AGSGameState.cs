﻿using System;
using System.Linq;
using AGS.API;
using System.Collections.Generic;
using Autofac;
using ProtoBuf;

namespace AGS.Engine
{
	public class AGSGameState : IGameState
	{
		private Lazy<ICutscene> _cutscene;

		public AGSGameState (IPlayer player, ICustomProperties globalVariables, Resolver resolver)
		{
			Rooms = new List<IRoom> ();
			UI = new AGSConcurrentHashSet<IObject> ();
			Player = player;
			GlobalVariables = globalVariables;
			_cutscene = new Lazy<ICutscene> (() => resolver.Container.Resolve<ICutscene>());
		}

		#region IGameState implementation

		public IPlayer Player { get; set; }

		public IList<IRoom> Rooms { get; private set; }

		public IConcurrentHashSet<IObject> UI { get; private set; }

		public ICustomProperties GlobalVariables { get; private set; }

		public ICutscene Cutscene { get { return _cutscene.Value; } }

		public bool Paused { get; set; }

		#endregion

		public void CopyFrom(IGameState state)
		{
			clean();
			Rooms = state.Rooms;
			Player = state.Player;
			UI = state.UI;
			GlobalVariables.CopyFrom(state.GlobalVariables);
			Cutscene.CopyFrom(state.Cutscene);
		}

		public TObject Find<TObject>(string id) where TObject : class, IObject
		{
			//Naive implementation, if this becomes a bottleneck, we'll need to maintain a dictionary of all objects
			if (typeof(TObject) == typeof(IObject) || typeof(TObject) == typeof(ICharacter))
			{
				return findInRooms<TObject>(id) ?? findUi<TObject>(id);
			}
			else
			{
				return findUi<TObject>(id) ?? findInRooms<TObject>(id);
			}
		}
			
		private TObject findUi<TObject>(string id) where TObject : class, IObject
		{
			return (UI.FirstOrDefault(o => o.ID == id)) as TObject;
		}

		private TObject findInRooms<TObject>(string id) where TObject : class, IObject
		{
			return (Rooms.SelectMany(r => r.Objects).FirstOrDefault(o => o.ID == id)) as TObject;
		}

		private void clean()
		{
			foreach (var room in Rooms)
			{
				room.Dispose();
			}
		}
	}
}

