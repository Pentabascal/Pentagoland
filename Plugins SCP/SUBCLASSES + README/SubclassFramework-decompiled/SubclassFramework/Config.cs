using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using UnityEngine;

namespace SubclassFramework;

public sealed class Config : IConfig
{
	[Description("Activa o desactiva el plugin entero.")]
	public bool IsEnabled { get; set; } = true;

	[Description("Mensajes extra en consola para depurar la asignacion de subclases.")]
	public bool Debug { get; set; } = false;

	[Description("Muestra un aviso superior con el nombre y la descripcion de la subclase al spawnear.")]
	public bool ShowSubclassOnSpawn { get; set; } = true;

	[Description("Duracion maxima (segundos) del banner de bienvenida. El minimo es 5s y se ajusta segun el numero de palabras.")]
	public float SpawnHintDuration { get; set; } = 6f;

	[Description("Muestra el contador de habilidad en pantalla. Desactivalo si choca con otro plugin de hints.")]
	public bool ShowAbilityHud { get; set; } = true;

	[Description("Desplazamiento vertical del contador de habilidad. 0 es la posicion normal y 3 lo baja claramente hacia la parte inferior.")]
	public int AbilityHudOffset { get; set; } = 3;

	[Description("Registra una tecla configurable por el jugador para las habilidades.")]
	public bool UseServerSpecificKeybind { get; set; } = false;

	[Description("Tecla sugerida por defecto para la habilidad (el jugador puede cambiarla). Valores validos: nombres de UnityEngine.KeyCode, por ejemplo X, G, V, Alpha5.")]
	public string DefaultAbilityKey { get; set; } = "H";

	[Description("Activa el módulo de SCP-173 (señuelo, avisos de Tantrum y variantes). Es la parte más experimental: si da problemas, ponlo en false.")]
	public bool EnableScp173Module { get; set; } = true;

	[Description("Activa el bot señuelo de SCP-173. Requiere EnableScp173Module.")]
	public bool EnableScp173Decoy { get; set; } = true;

	[Description("Activa las subclases de SCPs (939, 049, 049-2, 106).")]
	public bool EnableScpSubclasses { get; set; } = true;

	[Description("Activa el paquete de subclases extra (Radiotelegrafista, Bioquímico, Soplón, Escudero y variantes de SCP-096).")]
	public bool EnableExtraSubclasses { get; set; } = true;

	[Description("Curacion que recibe el SCP-096 por cada kill. 0 para desactivarlo.")]
	public float Scp096HealPerKill { get; set; } = 50f;

	[Description("Probabilidad base para humanos de recibir una subclase, incluso cuando el pool es pequeno.")]
	public float BaseSubclassChance { get; set; } = 30f;

	[Description("Probabilidad para humanos cuando hay exactamente 2 opciones disponibles.")]
	public float TwoOptionSubclassChance { get; set; } = 45f;

	[Description("Aumento de probabilidad por cada opcion adicional a partir de la segunda.")]
	public float SubclassChancePerOption { get; set; } = 3f;

	[Description("Limite maximo de probabilidad para humanos por pool.")]
	public float MaxSubclassChance { get; set; } = 80f;

	[Description("Probabilidad base de los SCP para recibir una subclase. Los SCP tienen mas opciones jugables y usan una probabilidad separada.")]
	public float ScpBaseSubclassChance { get; set; } = 55f;

	[Description("Probabilidad de los SCP cuando hay exactamente 2 opciones disponibles.")]
	public float ScpTwoOptionSubclassChance { get; set; } = 70f;

	[Description("Limite maximo de probabilidad de subclase para los SCP.")]
	public float ScpMaxSubclassChance { get; set; } = 92f;

	[Description("Desactivar subclases concretas por su ID. Ej: demoledor: false")]
	public Dictionary<string, bool> DisabledSubclasses { get; set; } = new Dictionary<string, bool>();

	[Description("Sobrescribe el peso de rareza (0-100) de una subclase por su ID. Un valor mayor la hace más frecuente dentro de su pool. Las subclases SCP mantienen siempre un peso igual.")]
	public Dictionary<string, float> ChanceOverrides { get; set; } = new Dictionary<string, float>();

	[Description("Sobrescribe el cooldown en segundos de una habilidad por su ID. Ej: veterano: 90")]
	public Dictionary<string, float> CooldownOverrides { get; set; } = new Dictionary<string, float>();

	[Description("Sobrescribe la vida maxima de una subclase por su ID. Ej: heavy: 115")]
	public Dictionary<string, int> HealthOverrides { get; set; } = new Dictionary<string, int>();

	public void Normalize()
	{
		SpawnHintDuration = ClampFinite(SpawnHintDuration, 6f, 5f, 30f);
		AbilityHudOffset = Mathf.Clamp(AbilityHudOffset, 0, 3);
		Scp096HealPerKill = Mathf.Max(0f, float.IsNaN(Scp096HealPerKill) || float.IsInfinity(Scp096HealPerKill)
			? 50f
			: Scp096HealPerKill);
		BaseSubclassChance = ClampFinite(BaseSubclassChance, 30f, 20f, 85f);
		TwoOptionSubclassChance = ClampFinite(TwoOptionSubclassChance, 45f, BaseSubclassChance, 90f);
		SubclassChancePerOption = ClampFinite(SubclassChancePerOption, 3f, 0f, 100f);
		MaxSubclassChance = ClampFinite(MaxSubclassChance, 80f, TwoOptionSubclassChance, 90f);
		ScpBaseSubclassChance = ClampFinite(ScpBaseSubclassChance, 55f, BaseSubclassChance, 95f);
		ScpTwoOptionSubclassChance = ClampFinite(ScpTwoOptionSubclassChance, 70f, ScpBaseSubclassChance, 98f);
		ScpMaxSubclassChance = ClampFinite(ScpMaxSubclassChance, 92f, ScpTwoOptionSubclassChance, 99f);

		DisabledSubclasses ??= new Dictionary<string, bool>();
		ChanceOverrides ??= new Dictionary<string, float>();
		CooldownOverrides ??= new Dictionary<string, float>();
		HealthOverrides ??= new Dictionary<string, int>();

		if (ChanceOverrides.Count > 0)
		{
			foreach (string item in new List<string>(ChanceOverrides.Keys))
			{
				ChanceOverrides[item] = ClampFinite(ChanceOverrides[item], 0f, 0f, 100f);
			}
		}

		if (CooldownOverrides.Count > 0)
		{
			foreach (string item2 in new List<string>(CooldownOverrides.Keys))
			{
				CooldownOverrides[item2] = ClampFinite(CooldownOverrides[item2], 0f, 0f, 3600f);
			}
		}

		foreach (string item3 in new List<string>(HealthOverrides.Keys))
		{
			HealthOverrides[item3] = Mathf.Max(1, HealthOverrides[item3]);
		}
	}

	private static float ClampFinite(float value, float fallback, float min, float max)
	{
		if (float.IsNaN(value) || float.IsInfinity(value))
		{
			value = fallback;
		}

		return Mathf.Clamp(value, min, max);
	}
}
