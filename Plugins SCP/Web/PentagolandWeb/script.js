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
