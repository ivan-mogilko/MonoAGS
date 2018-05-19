﻿using AGS.API;

namespace AGS.Engine
{
    public class AGSSkinComponent : AGSComponent, ISkinComponent
    {
        private IEntity _entity;

        public AGSSkinComponent(IGameSettings settings)
        {
            Skin = settings.Defaults.Skin;
            SkinTags = new AGSConcurrentHashSet<string>();
        }

        public ISkin Skin { get; set; }

        public IConcurrentHashSet<string> SkinTags { get; private set; }

        public override void Init(IEntity entity)
        {
            base.Init(entity);
            _entity = entity;
            entity.OnComponentsInitialized.Subscribe(onComponentsInitialized);
        }

        public override void Dispose()
        {
            base.Dispose();
            _entity = null;
        }

        private void onComponentsInitialized()
        {
            var skin = Skin;
            if (skin == null) return;
            skin.Apply(_entity);
        }
    }
}
