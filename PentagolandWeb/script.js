document.documentElement.classList.add('has-js');

const navToggle = document.querySelector('.nav-toggle');
const mainNav = document.querySelector('.main-nav');
const navLinks = Array.from(document.querySelectorAll('.main-nav a[href^="#"]'));
const year = document.querySelector('#year');
const revealItems = document.querySelectorAll('.reveal');
const contactForm = document.querySelector('#contact-form');
const formStatus = document.querySelector('#form-status');

if (year) {
  year.textContent = new Date().getFullYear();
}

if (navToggle && mainNav) {
  navToggle.addEventListener('click', () => {
    const isOpen = mainNav.classList.toggle('open');
    navToggle.setAttribute('aria-expanded', String(isOpen));
    navToggle.setAttribute('aria-label', isOpen ? 'Cerrar menú' : 'Abrir menú');
  });

  mainNav.querySelectorAll('a').forEach((link) => {
    link.addEventListener('click', () => {
      mainNav.classList.remove('open');
      navToggle.setAttribute('aria-expanded', 'false');
      navToggle.setAttribute('aria-label', 'Abrir menú');
    });
  });

  document.addEventListener('keydown', (event) => {
    if (event.key === 'Escape' && mainNav.classList.contains('open')) {
      mainNav.classList.remove('open');
      navToggle.setAttribute('aria-expanded', 'false');
      navToggle.setAttribute('aria-label', 'Abrir menú');
      navToggle.focus();
    }
  });

  document.addEventListener('click', (event) => {
    if (
      mainNav.classList.contains('open') &&
      event.target instanceof Node &&
      !mainNav.contains(event.target) &&
      !navToggle.contains(event.target)
    ) {
      mainNav.classList.remove('open');
      navToggle.setAttribute('aria-expanded', 'false');
      navToggle.setAttribute('aria-label', 'Abrir menú');
    }
  });
}

if ('IntersectionObserver' in window) {
  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          entry.target.classList.add('visible');
          observer.unobserve(entry.target);
        }
      });
    },
    { threshold: 0.12 }
  );

  revealItems.forEach((item) => observer.observe(item));
} else {
  revealItems.forEach((item) => item.classList.add('visible'));
}

if ('IntersectionObserver' in window && navLinks.length > 0) {
  const navSections = navLinks
    .map((link) => document.querySelector(link.getAttribute('href')))
    .filter((section) => section instanceof HTMLElement);
  const visibleSections = new Map();

  const updateCurrentSection = () => {
    const currentSection = Array.from(visibleSections.entries())
      .sort((left, right) => right[1] - left[1])[0];

    navLinks.forEach((link) => {
      const isCurrent = currentSection && link.getAttribute('href') === `#${currentSection[0]}`;
      link.classList.toggle('is-current', Boolean(isCurrent));

      if (isCurrent) {
        link.setAttribute('aria-current', 'location');
      } else {
        link.removeAttribute('aria-current');
      }
    });
  };

  const sectionObserver = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          visibleSections.set(entry.target.id, entry.intersectionRatio);
        } else {
          visibleSections.delete(entry.target.id);
        }
      });

      updateCurrentSection();
    },
    {
      rootMargin: '-20% 0px -65% 0px',
      threshold: [0, 0.2, 0.5, 1],
    }
  );

  navSections.forEach((section) => sectionObserver.observe(section));
}

if (contactForm && formStatus) {
  contactForm.addEventListener('submit', (event) => {
    event.preventDefault();

    if (!contactForm.reportValidity()) {
      return;
    }

    formStatus.textContent =
      'Los campos están completos, pero el formulario aún no tiene un destino configurado. No se ha enviado ni guardado nada.';
    formStatus.classList.add('is-notice');
  });
}

const serverStatusElements = {
  card: document.querySelector('#server-state'),
  label: document.querySelector('#server-state-label'),
  playerCount: document.querySelector('#server-player-count'),
  playerCapacity: document.querySelector('#server-player-capacity'),
  playerTotal: document.querySelector('.server-player-total'),
  listCount: document.querySelector('#server-list-count'),
  playerList: document.querySelector('#server-player-list'),
  summaryNote: document.querySelector('#server-summary-note'),
  updatedAt: document.querySelector('#server-updated-at'),
};

const hasServerStatusSection = Object.values(serverStatusElements).every(Boolean);

function updateServerStatus(snapshot) {
  if (!hasServerStatusSection) {
    throw new Error('No se encontró la sección Estado Servidores en la página.');
  }

  if (!snapshot || typeof snapshot !== 'object' || Array.isArray(snapshot)) {
    throw new TypeError('El estado debe ser un objeto con online, maxPlayers y players.');
  }

  if (typeof snapshot.online !== 'boolean') {
    throw new TypeError('online debe ser true o false.');
  }

  if (!Number.isInteger(snapshot.maxPlayers) || snapshot.maxPlayers < 1 || snapshot.maxPlayers > 200) {
    throw new RangeError('maxPlayers debe ser un entero entre 1 y 200.');
  }

  if (!Array.isArray(snapshot.players)) {
    throw new TypeError('players debe ser una lista de nombres de jugadores.');
  }

  const players = snapshot.players.map((playerName) => {
    if (typeof playerName !== 'string' || !playerName.trim() || playerName.trim().length > 32) {
      throw new TypeError('Cada nombre debe tener entre 1 y 32 caracteres.');
    }

    return playerName.trim();
  });

  if (players.length > snapshot.maxPlayers) {
    throw new RangeError('La lista de jugadores no puede superar maxPlayers.');
  }

  if (!snapshot.online && players.length > 0) {
    throw new Error('Un servidor desconectado no puede mostrar jugadores conectados.');
  }

  const updatedAt = snapshot.updatedAt == null ? new Date() : new Date(snapshot.updatedAt);
  if (Number.isNaN(updatedAt.getTime())) {
    throw new TypeError('updatedAt debe ser una fecha válida o una cadena ISO.');
  }

  const elements = serverStatusElements;
  elements.card.classList.remove('is-preview', 'is-online', 'is-offline');
  elements.card.classList.add(snapshot.online ? 'is-online' : 'is-offline');
  elements.label.textContent = snapshot.online ? 'En línea' : 'Sin conexión';
  elements.playerCount.textContent = String(players.length);
  elements.playerCapacity.textContent = String(snapshot.maxPlayers);
  elements.playerTotal.setAttribute(
    'aria-label',
    `${players.length} de ${snapshot.maxPlayers} jugadores conectados`
  );
  elements.listCount.textContent =
    players.length === 1 ? '1 conectado' : `${players.length} conectados`;
  elements.summaryNote.textContent = snapshot.online
    ? 'Estado recibido desde una integración del servidor. La información puede tener unos segundos de retraso.'
    : 'La integración indica que el servidor no está disponible en este momento.';
  elements.updatedAt.textContent = `Última actualización: ${updatedAt.toLocaleString('es-ES')}`;
  elements.playerList.replaceChildren();

  if (players.length === 0) {
    const emptyState = document.createElement('li');
    emptyState.className = 'server-empty-state';
    emptyState.textContent = snapshot.online
      ? 'No hay jugadores conectados ahora mismo.'
      : 'La lista no está disponible mientras el servidor está desconectado.';
    elements.playerList.append(emptyState);
    return;
  }

  players.forEach((playerName) => {
    const listItem = document.createElement('li');
    listItem.textContent = playerName;
    elements.playerList.append(listItem);
  });
}

if (hasServerStatusSection) {
  window.PentagolandServerStatus = Object.freeze({
    update: updateServerStatus,
  });
}
