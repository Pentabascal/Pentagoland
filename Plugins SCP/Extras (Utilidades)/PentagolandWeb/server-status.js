(() => {
  const MAX_STATUS_AGE_MS = readBoundedSetting('serverStatusMaxAgeSeconds', 120, 15, 3600) * 1000;
  const REFRESH_INTERVAL_MS = readBoundedSetting('serverStatusRefreshSeconds', 30, 15, 300) * 1000;
  const endpoint = document.documentElement.dataset.serverStatusEndpoint?.trim() ?? '';
  const home = {
    card: document.querySelector('#server-state'),
    label: document.querySelector('#server-state-label'),
    playerCount: document.querySelector('#server-player-count'),
    capacity: document.querySelector('#server-player-capacity'),
    total: document.querySelector('.server-player-total'),
    listCount: document.querySelector('#server-list-count'),
    list: document.querySelector('#server-player-list'),
    summary: document.querySelector('#server-summary-note'),
    updated: document.querySelector('#server-updated-at'),
    pendingClass: 'is-preview',
    fallbackLabel: 'Estado no disponible',
  };
  const panel = {
    card: document.querySelector('#panel-server-state'),
    label: document.querySelector('#panel-server-state-label'),
    playerCount: document.querySelector('#panel-server-player-count'),
    capacity: document.querySelector('#panel-server-player-capacity'),
    listCount: document.querySelector('#panel-server-list-count'),
    list: document.querySelector('#panel-server-player-list'),
    summary: document.querySelector('#panel-server-summary'),
    updated: document.querySelector('#panel-server-updated'),
    pendingClass: 'is-pending',
    fallbackLabel: 'Estado no disponible',
  };

  function readBoundedSetting(name, fallback, minimum, maximum) {
    const value = Number(document.documentElement.dataset[name]);
    return Number.isInteger(value) ? Math.min(maximum, Math.max(minimum, value)) : fallback;
  }

  function readField(snapshot, lowerName, upperName) {
    return snapshot[lowerName] ?? snapshot[upperName];
  }

  function parseTimestamp(value) {
    if (value instanceof Date) {
      return new Date(value.getTime());
    }

    if (typeof value !== 'string') {
      throw new TypeError('updatedAt debe ser una fecha ISO válida.');
    }

    const dotNetDate = /^\/Date\((-?\d+)(?:[+-]\d+)?\)\/$/.exec(value);
    const parsed = dotNetDate ? new Date(Number(dotNetDate[1])) : new Date(value);
    if (Number.isNaN(parsed.getTime())) {
      throw new TypeError('updatedAt debe ser una fecha ISO válida.');
    }

    return parsed;
  }

  function normalizeSnapshot(snapshot) {
    if (!snapshot || typeof snapshot !== 'object' || Array.isArray(snapshot)) {
      throw new TypeError('El estado del servidor debe ser un objeto JSON.');
    }

    const schemaVersion = readField(snapshot, 'schemaVersion', 'SchemaVersion');
    if (schemaVersion != null && schemaVersion !== 1) {
      throw new RangeError(`Versión de estado no compatible: ${schemaVersion}.`);
    }

    const online = readField(snapshot, 'online', 'Online');
    const onlinePlayers = readField(snapshot, 'onlinePlayers', 'OnlinePlayers');
    const maxPlayers = readField(snapshot, 'maxPlayers', 'MaxPlayers');
    const updatedAt = readField(snapshot, 'updatedAt', 'UpdatedAtUtc')
      ?? snapshot.GeneratedAtUtc;

    if (typeof online !== 'boolean') {
      throw new TypeError('online debe ser true o false.');
    }
    if (!Number.isInteger(onlinePlayers) || onlinePlayers < 0) {
      throw new RangeError('onlinePlayers debe ser un entero no negativo.');
    }
    if (!Number.isInteger(maxPlayers) || maxPlayers < 1 || maxPlayers > 200) {
      throw new RangeError('maxPlayers debe ser un entero entre 1 y 200.');
    }
    if (onlinePlayers > maxPlayers) {
      throw new RangeError('onlinePlayers no puede superar maxPlayers.');
    }
    if (!online && onlinePlayers !== 0) {
      throw new Error('Un servidor desconectado no puede mostrar jugadores conectados.');
    }

    const timestamp = parseTimestamp(updatedAt);
    const ageMs = Date.now() - timestamp.getTime();
    if (ageMs < -60_000) {
      throw new RangeError('updatedAt está demasiado adelantada respecto al reloj actual.');
    }

    return {
      online,
      onlinePlayers,
      maxPlayers,
      serverName: readField(snapshot, 'serverName', 'ServerName'),
      timestamp,
      stale: ageMs > MAX_STATUS_AGE_MS,
    };
  }

  function setListMessage(elements, message) {
    if (!elements.list) {
      return;
    }

    const item = document.createElement('li');
    item.className = elements.list === home.list ? 'server-empty-state' : 'live-player-empty';
    item.textContent = message;
    elements.list.replaceChildren(item);
  }

  function renderUnavailable(elements, label, message, timestamp = null) {
    if (!elements.card) {
      return;
    }

    elements.card.classList.remove('is-online', 'is-offline');
    elements.card.classList.add(elements.pendingClass);
    elements.label.textContent = label;
    elements.playerCount.textContent = '—';
    elements.capacity.textContent = '—';
    elements.listCount.textContent = 'Sin datos';
    elements.summary.textContent = message;
    elements.updated.textContent = timestamp
      ? `Último dato recibido: ${timestamp.toLocaleString('es-ES')}`
      : 'Última actualización: —';
    if (elements.total) {
      elements.total.setAttribute('aria-label', 'Estado del servidor no disponible');
    }
    setListMessage(elements, 'No se publica la lista de nombres de jugadores.');
  }

  function render(elements, rawSnapshot) {
    if (!elements.card) {
      throw new Error('No se encontró el contenedor del estado del servidor.');
    }

    const snapshot = normalizeSnapshot(rawSnapshot);
    if (snapshot.stale) {
      renderUnavailable(
        elements,
        'Datos desactualizados',
        'El último dato supera el límite de antigüedad; no se presenta como estado actual.',
        snapshot.timestamp,
      );
      return;
    }

    elements.card.classList.remove(elements.pendingClass, 'is-online', 'is-offline');
    elements.card.classList.add(snapshot.online ? 'is-online' : 'is-offline');
    elements.label.textContent = snapshot.online ? 'En línea' : 'Sin conexión';
    elements.playerCount.textContent = String(snapshot.onlinePlayers);
    elements.capacity.textContent = String(snapshot.maxPlayers);
    elements.listCount.textContent = 'Solo contador';
    elements.summary.textContent = snapshot.online
      ? 'Estado recibido desde la integración del servidor. Solo se comparte el número de jugadores, no sus nombres ni identificadores.'
      : 'La integración indica que el servidor no está disponible en este momento.';
    elements.updated.textContent = `Última actualización: ${snapshot.timestamp.toLocaleString('es-ES')}`;
    if (elements.total) {
      elements.total.setAttribute(
        'aria-label',
        `${snapshot.onlinePlayers} de ${snapshot.maxPlayers} jugadores conectados`,
      );
    }
    setListMessage(
      elements,
      snapshot.online
        ? 'Por privacidad, la web muestra el contador y no los nombres.'
        : 'La lista no está disponible mientras el servidor está desconectado.',
    );
  }

  function createStatusApi(elements) {
    return Object.freeze({
      update: (snapshot) => render(elements, snapshot),
      refresh: () => refresh(elements),
    });
  }

  async function refresh(elements) {
    if (!endpoint) {
      throw new Error('No se ha configurado data-server-status-endpoint en el documento HTML.');
    }

    let url;
    try {
      url = new URL(endpoint, document.baseURI);
    } catch (error) {
      throw new TypeError('El endpoint de estado no es una URL válida.', { cause: error });
    }

    if (!['http:', 'https:'].includes(url.protocol) || url.username || url.password) {
      throw new TypeError('El endpoint debe usar HTTP(S) y no puede incluir credenciales.');
    }

    const response = await fetch(url, {
      headers: { Accept: 'application/json' },
      cache: 'no-store',
      credentials: 'omit',
    });
    if (!response.ok) {
      throw new Error(`El endpoint de estado respondió HTTP ${response.status}.`);
    }

    render(elements, await response.json());
  }

  if (home.card) {
    window.PentagolandServerStatus = createStatusApi(home);
  }
  if (panel.card) {
    window.PentagolandPanelServerStatus = createStatusApi(panel);
  }

  if (endpoint) {
    const refreshAll = async () => {
      for (const elements of [home, panel]) {
        if (!elements.card) {
          continue;
        }

        try {
          await refresh(elements);
        } catch (error) {
          console.error('No se pudo actualizar el estado de Pentagoland:', error);
          renderUnavailable(
            elements,
            'Estado no disponible',
            'No se pudo consultar la fuente de estado. Se reintentará automáticamente.',
          );
        }
      }

      window.setTimeout(refreshAll, REFRESH_INTERVAL_MS);
    };

    refreshAll();
  }
})();
