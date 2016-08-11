﻿namespace AGS.API
{
    public interface IScalingArea : IArea
	{
		float MinScaling { get; set; }
		float MaxScaling { get; set; }
		bool ScaleObjects { get; set; }
		bool ZoomCamera { get; set; }
		bool ScaleVolume { get; set; }
        
        float GetScaling(float value);
	}
}
