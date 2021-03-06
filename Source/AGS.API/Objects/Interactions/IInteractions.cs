﻿namespace AGS.API
{
    /// <summary>
    /// Allows subscribing interaction events (to define what happens when an object is looked at/interacted with/etc). 
    /// </summary>
    public interface IInteractions
	{
        /// <summary>
        /// The event for when interacting with an object.
        /// The available verbs depend on your control scheme. For example, you might have a separate icon for talk
        /// in your game which is different from the interact icon.
        /// There's also a "Default" event which will be used if no specific event was subscribed to.
        /// There's a chain of default behaviors that can be defined to allow for generic responses.
        /// The event handler lookup is as follows: 
        /// 1. Object's verb
        /// 2. Object's default verb
        /// 3. IGame.Events.DefaultInteractions verb
        /// 4. IGame.Events.DefaultInteractions default verb
        /// </summary>
        /// <value>
        /// The interaction event.
        /// </value>
        /// <example>
        /// <code language="lang-csharp">
        /// public void SubscribeEvents()
        /// {
        ///     game.Events.DefaultInteractions.OnInteract("Talk").Subscribe(onDefaultTalk);
        ///     oTeapot.Interactions.OnInteract(AGSInteractions.Look).Subscribe(onTeapotLook);
        ///     oTeapot.Interactions.OnInteract(AGSInteractions.Interact).SubscribeToAsync(onTeapotInteract);
        ///     oTeapot.Interactions.OnInteract("Throw").Subscribe(onTeapotThrow);
        ///     oTeapot.Interactions.OnInteract(AGSInteractions.Default).Subscribe(onTeapotDefault);
        /// }
        /// 
        /// private async void onDefaultTalk(ObjectEventArgs args)
        /// {
        ///     await cEgo.SayAsync(string.Format("{0}? No, I don't think it's going to talk back.", args.Object.Hotspot));
        /// }
        /// 
        /// private async void onTeapotLook(ObjectEventArgs args)
        /// {
        ///     await cEgo.SayAsync("What a lovely looking teapot!");
        /// }
        /// 
        /// private async Task onTeapotInteract(ObjectEventArgs args)
        /// {
        ///     await cEgo.SayAsync("I'm going to pour some tea now.");
        ///     oTeapot.StartAnimation(aPourTea);
        /// }
        /// 
        /// private async void onTeapotThrow(ObjectEventArgs args)
        /// {
        ///     await cEgo.SayAsync("No way, I'm not throwing the teapot.");
        /// }
        /// 
        /// private async void onTeapotDefault(ObjectEventArgs args)
        /// {
        ///     await cEgo.SayAsync("I'm not doing anything like that to the teapot.");
        /// }
        /// </code>
        /// </example>
        IEvent<ObjectEventArgs> OnInteract(string verb);

        /// <summary>
        /// The event for when using an inventory item on an object.
        /// The available verbs depend on your control scheme. For example, you might have a separate icon for give
        /// item in your game which is different from the interact icon.
        /// There's also a "Default" event which will be used if no specific event was subscribed to.
        /// There's a chain of default behaviors that can be defined to allow for generic responses.
        /// The event handler lookup is as follows: 
        /// 1. Object's verb
        /// 2. Object's default verb
        /// 3. IGame.Events.DefaultInteractions verb
        /// 4. IGame.Events.DefaultInteractions default verb
        /// </summary>
        /// <value>
        /// The inventory interaction event.
        /// </value>
        /// <example>
        /// <code language="lang-csharp">
        /// public void SubscribeEvents()
        /// {
        ///     oTeapot.Interactions.OnInventoryInteract(AGSInteractions.Interact).Subscribe(onTeapotInventoryInteract);
        ///     game.Events.DefaultInteractions.OnInventoryInteract(AGSInteractions.Interact).Subscribe(onDefaultInventoryInteract);
        /// }
        /// 
        /// private async void onTeapotInventoryInteract(InventoryInteractEventArgs args)
        /// {
        ///     if (args.Item == iCup)
        ///     {
        ///         await cEgo.SayAsync("Ok, I'm going to pour tea in the cup");
        ///         oTeapot.StartAnimation(aPourTea);
        ///         cEgo.Inventory.Items.Remove(iCup);
        ///         cEgo.Inventory.Items.Add(iFullCup);
        ///     }
        ///     else if (args.Item == iFullCup)
        ///     {
        ///         await cEgo.SayAsync("The cup is already full.");
        ///     }
        ///     else
        ///     {
        ///         await cEgo.SayAsync("This is not what you'd usually use on a teapot...");
        ///     }
        /// }
        /// 
        /// private async void onDefaultInventoryInteract(InventoryInteractEventArgs args)
        /// {
        ///     await cEgo.SayAsync(string.Format("Using {0} on {1}? No, I don't think so.", args.Item.Graphics.Hotspot, args.Object.Hotspot));
        /// }
        /// </code>
        /// </example>
        IEvent<InventoryInteractEventArgs> OnInventoryInteract(string verb);
	}
}

