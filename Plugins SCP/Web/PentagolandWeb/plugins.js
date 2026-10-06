const plugins = [
  { name: 'RueI', platform: 'LabAPI', version: '3.1.2.0', category: 'Interfaz', status: 'active' },
  { name: 'SecretAPI', platform: 'LabAPI', version: '3.4.0.0', category: 'Infraestructura', status: 'active' },
  { name: 'SCP Proximity Chat', platform: 'LabAPI', version: '1.1.1.0', category: 'Voz', status: 'active' },
  { name: 'SecretLabNAudio.FFmpeg', platform: 'LabAPI', version: '2.1.0.0', category: 'Infraestructura', status: 'active' },
  { name: 'SecretLabNAudio', platform: 'LabAPI', version: '2.1.0.0', category: 'Infraestructura', status: 'active' },
  { name: 'AntiSuicidalZombies', platform: 'LabAPI', version: '3.0.3', category: 'Juego', status: 'active' },
  { name: 'LateSpawn', platform: 'LabAPI', version: '2.0.0', category: 'Juego', status: 'active' },
  { name: 'Timers', platform: 'LabAPI', version: '1.5.1', category: 'Utilidades', status: 'active' },
  { name: 'MvpSystem', platform: 'LabAPI', version: '1.2.0', category: 'Estadísticas', status: 'active' },
  { name: 'ProjectMER', platform: 'LabAPI', version: '2026.7.6.1', category: 'Juego', status: 'active' },
  { name: 'RespawnTimer', platform: 'LabAPI', version: '1.5.0', category: 'Juego', status: 'active' },
  { name: 'SSSS Proximity Chat', platform: 'LabAPI', version: '1.1.1.0', category: 'Voz', status: 'active' },
  { name: 'SER', platform: 'LabAPI', version: '1.0.0', category: 'Eventos', status: 'active' },
  { name: 'StatsSystem', platform: 'LabAPI', version: '2.1.1', category: 'Estadísticas', status: 'active' },
  { name: 'Exiled.Events', platform: 'EXILED', version: '9.14.2', category: 'Infraestructura', status: 'active' },
  { name: 'Exiled.Permissions', platform: 'EXILED', version: '9.14.2', category: 'Administración', status: 'active' },
  { name: 'Exiled.CustomItems', platform: 'EXILED', version: '9.14.2', category: 'Personalización', status: 'active' },
  { name: 'Exiled.CustomRoles', platform: 'EXILED', version: '9.14.2', category: 'Personalización', status: 'active' },
  { name: 'Exiled.CreditTags', platform: 'EXILED', version: '9.14.2', category: 'Créditos y etiquetas', status: 'active' },
  { name: 'Admin Tools', platform: 'EXILED', version: '8.0.2', category: 'Administración', status: 'active' },
  { name: 'Common Utilities', platform: 'EXILED', version: '8.1.2', category: 'Utilidades', status: 'active' },
  { name: 'Pentagoland Core', platform: 'EXILED', version: '4.2.0', category: 'Integración', status: 'active' },
  { name: 'VVUP Custom Items Notification Patch', platform: 'EXILED', version: '1.0.0', category: 'Interfaz', status: 'active' },
  { name: 'ScpSwap', platform: 'EXILED', version: '1.3.0', category: 'Juego', status: 'active' },
  { name: 'RemoteKeycard', platform: 'EXILED', version: '3.4.2', category: 'Juego', status: 'active' },
  { name: 'VVUP Custom Item Spawn Manager', platform: 'EXILED', version: '1.0.0', category: 'Personalización', status: 'active' },
  { name: 'UltimateHUD', platform: 'EXILED', version: '7.1.1', category: 'Interfaz', status: 'active' },
  { name: 'SCPReplacer', platform: 'EXILED', version: '1.2.1', category: 'Juego', status: 'active' },
  { name: 'SpectatorListRueI', platform: 'EXILED', version: '0.1.0', category: 'Interfaz', status: 'active' },
  { name: 'ServerWatermark', platform: 'EXILED', version: '0.0.5', category: 'Interfaz', status: 'active' },
  { name: 'ScpsInfoDisplay', platform: 'EXILED', version: '3.0.0', category: 'Interfaz', status: 'active' },
  { name: 'VVUP Base', platform: 'EXILED', version: '3.6.7', category: 'Infraestructura', status: 'active' },
  { name: 'VVUP Custom Item RueI Bridge', platform: 'EXILED', version: '1.6.1', category: 'Interfaz', status: 'active' },
  { name: 'Subclass Inventory Overrides', platform: 'EXILED', version: '1.0.0', category: 'Juego', status: 'active' },
  { name: 'SubclassFramework', platform: 'EXILED', version: '1.3.0', category: 'Infraestructura', status: 'active' },
  { name: 'VVUP Round Start', platform: 'EXILED', version: '4.0.0', category: 'Juego', status: 'active' },
  { name: 'VVUP SCP Changes', platform: 'EXILED', version: '3.6.7', category: 'Juego', status: 'active' },
  { name: 'VVUP Post Nuke Enhancements', platform: 'EXILED', version: '4.0.0', category: 'Juego', status: 'active' },
  { name: 'VVUP Micro Damage Reduction', platform: 'EXILED', version: '4.0.0', category: 'Juego', status: 'active' },
  { name: 'VVUP Server Events', platform: 'EXILED', version: '4.0.0', category: 'Eventos', status: 'active' },
  { name: 'VVUP Custom Items', platform: 'EXILED', version: '3.6.7', category: 'Personalización', status: 'active' },
  { name: 'VVUP Custom Escapes', platform: 'EXILED', version: '4.0.0', category: 'Juego', status: 'active' },
  { name: 'VVUP Credit Tags', platform: 'EXILED', version: '4.0.0', category: 'Créditos y etiquetas', status: 'active' },
  { name: 'VVUP Weapon Evaporate', platform: 'EXILED', version: '4.0.0', category: 'Juego', status: 'active' },
  { name: 'VVUP Operation Crossfire Server Event', platform: 'EXILED', version: '4.0.0', category: 'Eventos', status: 'active' },
  { name: 'VVUP Free Custom Roles', platform: 'EXILED', version: '4.0.0', category: 'Personalización', status: 'active' },
  { name: 'VVUP Husk Infection', platform: 'EXILED', version: '4.0.0', category: 'Juego', status: 'active' },
  { name: 'Axwabo.CommandSystem', platform: 'EXILED', version: '1.0.4', category: 'Utilidades', status: 'active' },
  { name: 'KillLogs', platform: 'EXILED', version: '—', category: 'Administración', status: 'disabled' },
  { name: 'UsefulHints', platform: 'EXILED', version: '—', category: 'Interfaz', status: 'disabled' },
  { name: 'UsefulHints.Addons', platform: 'EXILED', version: '—', category: 'Interfaz', status: 'disabled' },
  { name: 'CedMod', platform: 'LabAPI', version: '—', category: 'Administración', status: 'disabled' },
  { name: 'SCPSL Arcade Mode', platform: 'LabAPI', version: '—', category: 'Juego', status: 'disabled' },
  { name: 'Exiled Loader', platform: 'LabAPI', version: '9.14.2.0', category: 'Infraestructura', status: 'dependency' },
  { name: 'SCPDiscord', platform: 'Integración', version: '3.4.2 (código)', category: 'Discord', status: 'planned' },
];

const statusLabels = {
  active: 'Activo en el último arranque',
  disabled: 'Desactivado en configuración auditada',
  dependency: 'Capa de integración',
  planned: 'Pendiente de evaluación',
};

const searchInput = document.querySelector('#plugin-search');
const platformSelect = document.querySelector('#plugin-platform');
const categorySelect = document.querySelector('#plugin-category');
const statusSelect = document.querySelector('#plugin-status');
const resultContainer = document.querySelector('#plugin-results');
const emptyMessage = document.querySelector('#catalog-empty');
const resultCount = document.querySelector('#catalog-result-count');

function createPluginCard(plugin) {
  const card = document.createElement('article');
  card.className = `plugin-card plugin-card-${plugin.status}`;

  const top = document.createElement('div');
  top.className = 'plugin-card-top';

  const title = document.createElement('h3');
  title.textContent = plugin.name;

  const status = document.createElement('span');
  status.className = `plugin-status plugin-status-${plugin.status}`;
  status.textContent = statusLabels[plugin.status];
  top.append(title, status);

  const details = document.createElement('dl');
  details.className = 'plugin-metadata';
  for (const [label, value] of [
    ['Plataforma', plugin.platform],
    ['Versión', plugin.version],
    ['Categoría', plugin.category],
  ]) {
    const row = document.createElement('div');
    const term = document.createElement('dt');
    const description = document.createElement('dd');
    term.textContent = label;
    description.textContent = value;
    row.append(term, description);
    details.append(row);
  }

  card.append(top, details);
  return card;
}

function renderPlugins() {
  const search = searchInput.value.trim().toLocaleLowerCase('es');
  const platform = platformSelect.value;
  const category = categorySelect.value;
  const status = statusSelect.value;
  const filtered = plugins.filter((plugin) => {
    const matchesSearch = `${plugin.name} ${plugin.platform} ${plugin.category}`
      .toLocaleLowerCase('es')
      .includes(search);
    return (
      matchesSearch &&
      (!platform || plugin.platform === platform) &&
      (!category || plugin.category === category) &&
      (!status || plugin.status === status)
    );
  });

  resultContainer.replaceChildren(...filtered.map(createPluginCard));
  emptyMessage.hidden = filtered.length > 0;
  resultCount.textContent = `${filtered.length} ${filtered.length === 1 ? 'resultado' : 'resultados'}`;
}

const categories = [...new Set(plugins.map((plugin) => plugin.category))].sort((a, b) =>
  a.localeCompare(b, 'es'),
);
for (const category of categories) {
  const option = document.createElement('option');
  option.value = category;
  option.textContent = category;
  categorySelect.append(option);
}

document.querySelector('#active-count').textContent = String(
  plugins.filter((plugin) => plugin.status === 'active').length,
);
document.querySelector('#disabled-count').textContent = String(
  plugins.filter((plugin) => plugin.status === 'disabled').length,
);
document.querySelector('#planned-count').textContent = String(
  plugins.filter((plugin) => plugin.status === 'planned').length,
);

for (const control of [searchInput, platformSelect, categorySelect, statusSelect]) {
  control.addEventListener('input', renderPlugins);
  control.addEventListener('change', renderPlugins);
}

renderPlugins();
