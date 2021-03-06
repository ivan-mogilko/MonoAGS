﻿namespace AGS.API
{
    /// <summary>
    /// Represents the render loop of the game. All drawing to the screen is performed from the render loop.
    /// </summary>
    /// <seealso cref="IRenderPipeline"/>
    /// <seealso cref="IRenderer"/>
    /// <seealso cref="IRenderInstruction"/>
    public interface IRendererLoop
	{
        /// <summary>
        /// Calls the next tick, which triggers the next render.
        /// This is called from within the engine.
        /// </summary>
        void Tick();
	}
}