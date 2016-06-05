﻿using System;
using System.Collections.Generic;

namespace AGS.API
{
	public interface IRoom : IDisposable
	{
		string ID { get; }
		bool ShowPlayer { get; set; }
		IAudioClip MusicOnLoad { get; set; }
		IViewport Viewport { get; }
		IObject Background  { get; set; }
		IConcurrentHashSet<IObject> Objects { get; }
		IRoomEvents Events { get; }
		ICustomProperties Properties { get; }

		IList<IArea> WalkableAreas { get; }
		IList<IWalkBehindArea> WalkBehindAreas { get; }
		IList<IScalingArea> ScalingAreas { get; }
		IEdges Edges { get; }

		IEnumerable<IObject> GetVisibleObjectsFrontToBack(bool includeUi = true);
		IObject GetObjectAt(float x, float y, bool onlyEnabled = true, bool includeUi = true);
		TObject Find<TObject>(string id) where TObject : class, IObject;
	}
}

