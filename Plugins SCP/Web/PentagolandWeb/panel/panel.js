const viewButtons = Array.from(document.querySelectorAll('[data-view]'));
const views = Array.from(document.querySelectorAll('[data-panel-section]'));
const pageTitle = document.querySelector('#page-title');
const profileForm = document.querySelector('#profile-form');
const profileSaveStatus = document.querySelector('#profile-save-status');
const avatarInput = document.querySelector('#profile-avatar');
const avatarPreview = document.querySelector('#avatar-preview');
const profilePreviewAvatar = document.querySelector('#profile-preview-avatar');
const resetProfileButton = document.querySelector('#reset-profile');
const profileStorageKey = 'pentagoland-profile-draft-v1';
const steamIdStorageKey = 'pentagoland-steamid-v1';
const steamIdForm = document.querySelector('#steamid-form');
const steamIdInput = document.querySelector('#steamid-input');
const steamIdStatus = document.querySelector('#steamid-status');
const clearSteamIdButton = document.querySelector('#clear-steamid');
const steamIdActionLabel = document.querySelector('#steamid-action-label');
const steamIdSidebarTitle = document.querySelector('#steamid-sidebar-title');
const steamIdSidebarStatus = document.querySelector('#steamid-sidebar-status');
const steamIdOverviewStatus = document.querySelector('#steamid-overview-status');
const steamIdStatsValue = document.querySelector('#steamid-stats-value');
const steamProfileLink = document.querySelector('#steam-profile-link');
let savedSteamId = '';
let avatarDataUrl = '';

const viewTitles = {
  overview: 'Inicio',
  stats: 'Stat tracker',
  team: 'Equipo',
  profile: 'Mi perfil',
  account: 'Cuenta',
  server: 'Estado del servidor',
};

function selectView(selectedView, shouldScroll = true) {
  if (!Object.prototype.hasOwnProperty.call(viewTitles, selectedView)) {
    return;
  }

  viewButtons.forEach((button) => {
    const isActive = button.dataset.view === selectedView;
    button.classList.toggle('is-active', isActive);
    if (isActive) {
      button.setAttribute('aria-current', 'page');
    } else {
      button.removeAttribute('aria-current');
    }
  });

  views.forEach((view) => {
    view.hidden = view.dataset.panelSection !== selectedView;
  });

  pageTitle.textContent = viewTitles[selectedView];
  if (window.location.hash !== `#${selectedView}`) {
    window.location.hash = selectedView;
  }
  if (shouldScroll) {
    window.scrollTo(0, 0);
  }
}

viewButtons.forEach((button) => {
  button.addEventListener('click', () => selectView(button.dataset.view));
});

document.querySelectorAll('[data-open-view]').forEach((button) => {
  button.addEventListener('click', () => selectView(button.dataset.openView));
});

const initialView = window.location.hash.slice(1);
selectView(Object.prototype.hasOwnProperty.call(viewTitles, initialView) ? initialView : 'overview', false);

window.addEventListener('hashchange', () => {
  const requestedView = window.location.hash.slice(1);
  const activeView = document.querySelector('.panel-nav-link.is-active')?.dataset.view;
  if (
    Object.prototype.hasOwnProperty.call(viewTitles, requestedView) &&
    requestedView !== activeView
  ) {
    selectView(requestedView);
  }
});

function isValidSteamId(value) {
  return /^7656119\d{10}$/.test(value);
}

function renderSavedSteamId() {
  const hasSteamId = isValidSteamId(savedSteamId);

  steamIdSidebarTitle.textContent = hasSteamId ? 'SteamID guardado' : 'SteamID sin guardar';
  steamIdSidebarStatus.textContent = hasSteamId ? 'Solo en este navegador' : 'Se guarda solo aquí';
  steamIdOverviewStatus.textContent = hasSteamId ? 'SteamID guardado localmente' : 'Sin SteamID guardado';
  steamIdStatsValue.textContent = hasSteamId ? savedSteamId : 'Aún no has guardado uno';
  steamIdActionLabel.textContent = hasSteamId ? 'Cambiar SteamID' : 'Añadir SteamID';
  clearSteamIdButton.hidden = !savedSteamId;
  steamProfileLink.hidden = !hasSteamId;

  if (hasSteamId) {
    steamIdInput.value = savedSteamId;
    steamProfileLink.href = `https://steamcommunity.com/profiles/${savedSteamId}`;
  } else {
    steamProfileLink.removeAttribute('href');
  }
}

function loadSavedSteamId() {
  try {
    const storedValue = localStorage.getItem(steamIdStorageKey);
    if (!storedValue) {
      renderSavedSteamId();
      return;
    }

    if (!isValidSteamId(storedValue)) {
      savedSteamId = storedValue;
      renderSavedSteamId();
      steamIdStatus.textContent =
        'El valor guardado no tiene el formato esperado. Puedes introducir otro SteamID o quitar el guardado.';
      return;
    }

    savedSteamId = storedValue;
    renderSavedSteamId();
    steamIdStatus.textContent =
      'SteamID recordado en este navegador. No está verificado y todavía no carga estadísticas.';
  } catch (error) {
    steamIdStatus.textContent =
      'El navegador no permitió leer el SteamID guardado. Puedes probar a introducirlo de nuevo.';
    console.error('No se pudo leer el SteamID local:', error);
  }
}

steamIdForm.addEventListener('submit', (event) => {
  event.preventDefault();
  const candidate = steamIdInput.value.trim();

  if (!isValidSteamId(candidate)) {
    steamIdStatus.textContent = 'Introduce un SteamID64 de 17 cifras que empiece por 7656119.';
    steamIdInput.setAttribute('aria-invalid', 'true');
    steamIdInput.focus();
    return;
  }

  try {
    localStorage.setItem(steamIdStorageKey, candidate);
    savedSteamId = candidate;
    renderSavedSteamId();
    steamIdInput.setAttribute('aria-invalid', 'false');
    steamIdStatus.textContent =
      'Guardado en este navegador. No verifica que seas el titular ni consulta estadísticas.';
  } catch (error) {
    steamIdStatus.textContent =
      'No se pudo guardar. El navegador puede estar bloqueando el almacenamiento local.';
    console.error('No se pudo guardar el SteamID local:', error);
  }
});

steamIdInput.addEventListener('input', () => {
  steamIdInput.removeAttribute('aria-invalid');
  steamIdStatus.textContent = '';
});

clearSteamIdButton.addEventListener('click', () => {
  try {
    localStorage.removeItem(steamIdStorageKey);
    savedSteamId = '';
    steamIdInput.value = '';
    steamIdInput.removeAttribute('aria-invalid');
    renderSavedSteamId();
    steamIdStatus.textContent = 'SteamID quitado de este navegador.';
  } catch (error) {
    steamIdStatus.textContent = 'No se pudo quitar el SteamID guardado.';
    console.error('No se pudo borrar el SteamID local:', error);
  }
});

document.querySelectorAll('[data-steamid-focus]').forEach((button) => {
  button.addEventListener('click', () => {
    selectView('overview');
    steamIdForm.scrollIntoView({ behavior: 'smooth', block: 'center' });
    steamIdInput.focus({ preventScroll: true });
  });
});

document.querySelectorAll('[data-team-role]').forEach((button) => {
  button.addEventListener('click', () => {
    document.querySelectorAll('[data-team-role]').forEach((item) => {
      item.classList.toggle('is-selected', item === button);
    });

    const detail = document.querySelector('#team-detail');
    const role = button.dataset.teamRole;
    detail.querySelector('.eyebrow').textContent = role;
    detail.querySelector('h2').textContent = 'Perfiles por anunciar';
    detail.querySelector('p:last-child').textContent =
      `Todavía no hay perfiles públicos de ${role.toLowerCase()}. Cuando sus integrantes decidan compartirlos, podrás abrir aquí su bio y sus conexiones.`;
  });
});

function updateProfilePreview(profile) {
  const displayName = profile.displayName.trim();
  const description = profile.description.trim();
  const bio = profile.bio.trim();
  const steam = profile.steam.trim();
  const discord = profile.discord.trim();
  const initial = displayName ? Array.from(displayName)[0].toUpperCase() : '?';

  document.querySelector('#profile-preview-name').textContent = displayName || 'Tu nombre';
  document.querySelector('#profile-preview-description').textContent =
    description || 'Tu descripción corta aparecerá aquí.';
  document.querySelector('#profile-preview-bio').textContent =
    bio || 'Cuando escribas tu bio, podrás ver aquí cómo quedará.';

  const connections = [];
  if (steam) {
    connections.push(`Steam: ${steam}`);
  }
  if (discord) {
    connections.push(`Discord: ${discord}`);
  }
  document.querySelector('#profile-preview-connections').textContent =
    connections.length > 0 ? connections.join(' · ') : 'Aún no hay conexiones añadidas.';

  [avatarPreview, profilePreviewAvatar].forEach((element) => {
    element.replaceChildren();
    if (avatarDataUrl) {
      const image = document.createElement('img');
      image.src = avatarDataUrl;
      image.alt = '';
      element.append(image);
    } else {
      element.textContent = initial;
    }
  });
}

function readProfileForm() {
  const formData = new FormData(profileForm);
  return {
    displayName: String(formData.get('displayName') || ''),
    description: String(formData.get('description') || ''),
    bio: String(formData.get('bio') || ''),
    steam: String(formData.get('steam') || ''),
    discord: String(formData.get('discord') || ''),
  };
}

function loadProfileDraft() {
  try {
    const savedDraft = localStorage.getItem(profileStorageKey);
    if (!savedDraft) {
      updateProfilePreview(readProfileForm());
      return;
    }

    const draft = JSON.parse(savedDraft);
    if (!draft || typeof draft !== 'object' || Array.isArray(draft)) {
      throw new TypeError('El borrador guardado no tiene un formato válido.');
    }

    for (const field of ['displayName', 'description', 'bio', 'steam', 'discord']) {
      if (typeof draft[field] === 'string') {
        profileForm.elements.namedItem(field).value = draft[field];
      }
    }

    if (
      typeof draft.avatarDataUrl === 'string' &&
      /^data:image\/(?:png|jpeg|webp);base64,/.test(draft.avatarDataUrl)
    ) {
      avatarDataUrl = draft.avatarDataUrl;
    }

    updateProfilePreview(readProfileForm());
  } catch (error) {
    profileSaveStatus.textContent =
      'No se pudo leer el borrador local. Puedes borrarlo y empezar de nuevo.';
    console.error('No se pudo cargar el borrador del perfil:', error);
    updateProfilePreview(readProfileForm());
  }
}

profileForm.addEventListener('input', () => {
  updateProfilePreview(readProfileForm());
  profileSaveStatus.textContent = 'Vista previa actualizada. Recuerda guardar el borrador si quieres conservarlo en este navegador.';
});

avatarInput.addEventListener('change', () => {
  const [file] = avatarInput.files;
  if (!file) {
    return;
  }

  const allowedTypes = ['image/png', 'image/jpeg', 'image/webp'];
  if (!allowedTypes.includes(file.type)) {
    profileSaveStatus.textContent = 'Elige una imagen PNG, JPG o WebP.';
    avatarInput.value = '';
    return;
  }

  if (file.size > 300 * 1024) {
    profileSaveStatus.textContent = 'La imagen supera el límite de 300 KB.';
    avatarInput.value = '';
    return;
  }

  const reader = new FileReader();
  reader.addEventListener('load', () => {
    if (typeof reader.result !== 'string' || !reader.result.startsWith('data:image/')) {
      profileSaveStatus.textContent = 'No se pudo leer la imagen seleccionada.';
      return;
    }

    avatarDataUrl = reader.result;
    updateProfilePreview(readProfileForm());
    profileSaveStatus.textContent = 'Imagen lista para la vista previa. Guarda el borrador para conservarla en este navegador.';
  });
  reader.addEventListener('error', () => {
    profileSaveStatus.textContent = 'No se pudo leer la imagen seleccionada.';
  });
  reader.readAsDataURL(file);
});

profileForm.addEventListener('submit', (event) => {
  event.preventDefault();
  if (!profileForm.reportValidity()) {
    return;
  }

  try {
    const draft = { ...readProfileForm(), avatarDataUrl };
    localStorage.setItem(profileStorageKey, JSON.stringify(draft));
    profileSaveStatus.textContent =
      'Borrador guardado solo en este navegador. Aún no se ha publicado ni asociado a una cuenta.';
  } catch (error) {
    profileSaveStatus.textContent =
      'No se pudo guardar el borrador local. Prueba con una imagen más pequeña o borra datos del sitio.';
    console.error('No se pudo guardar el borrador del perfil:', error);
  }
});

resetProfileButton.addEventListener('click', () => {
  try {
    localStorage.removeItem(profileStorageKey);
  } catch (error) {
    profileSaveStatus.textContent = 'No se pudo borrar el borrador local.';
    console.error('No se pudo borrar el borrador del perfil:', error);
    return;
  }

  profileForm.reset();
  avatarDataUrl = '';
  updateProfilePreview(readProfileForm());
  profileSaveStatus.textContent = 'Borrador local borrado.';
});

loadSavedSteamId();
loadProfileDraft();
