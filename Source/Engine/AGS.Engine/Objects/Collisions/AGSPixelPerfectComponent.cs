﻿using AGS.API;

namespace AGS.Engine
{
    public class AGSPixelPerfectComponent : AGSComponent, IPixelPerfectComponent
    {
        private IImageComponent _image;

        public override void Init()
        {
            base.Init();

            Entity.Bind<IImageComponent>(c => { _image = c; }, _ => { _image = null; });
        }

        [Property(Category = "Collider")]
        public bool IsPixelPerfect { get; set; }

        [Property(Browsable = false)]
        public IArea PixelPerfectHitTestArea => _image?.CurrentSprite?.PixelPerfectHitTestArea;
    }
}
