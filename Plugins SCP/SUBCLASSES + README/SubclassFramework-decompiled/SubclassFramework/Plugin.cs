using System;
using Exiled.API.Features;
using SubclassFramework.Core;
using SubclassFramework.Subclasses;

namespace SubclassFramework;

public sealed class Plugin : Plugin<Config>
{
	public static Plugin Instance { get; private set; }

	public override string Name => "SubclassFramework";

	public override string Prefix => "subclass_framework";

	public override string Author => "calvoconpelo";

	public override Version Version => new Version(1, 3, 0);

	public override Version RequiredExiledVersion => new Version(9, 14, 2);

	public SubclassManager Manager { get; private set; }

	public override void OnEnabled()
	{
		Instance = this;
		try
		{
			Config.Normalize();
			Manager = new SubclassManager();
			Manager.Enable();
			AbilityInput.Register();
			if (Config.EnableScp173Module)
			{
				Scp173Module.Register();
			}
			base.OnEnabled();
		}
		catch
		{
			Scp173Module.Unregister();
			AbilityInput.Unregister();
			Manager?.Disable();
			Manager = null;
			Instance = null;
			throw;
		}
	}

	public override void OnDisabled()
	{
		AbilityInput.Unregister();
		Scp173Module.Unregister();
		Manager?.Disable();
		Manager = null;
		base.OnDisabled();
		Instance = null;
	}
}
