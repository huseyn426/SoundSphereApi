/* Never cache HTML/MVC responses — stale "/" broke auth UI. Network-first for static assets. */
const CACHE_NAME = 'soundsphere-v2';

const PRECACHE_URLS = [
  '/css/site.css',
  '/js/i18n.js',
  '/js/player.js',
  '/js/playlists-ui.js',
  '/js/track-row-menu.js'
];

self.addEventListener('install', (event) => {
  self.skipWaiting();
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) =>
      cache.addAll(PRECACHE_URLS).catch(() => {})
    )
  );
});

self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches
      .keys()
      .then((keys) =>
        Promise.all(keys.filter((k) => k !== CACHE_NAME).map((k) => caches.delete(k)))
      )
      .then(() => self.clients.claim())
  );
});

self.addEventListener('fetch', (event) => {
  const req = event.request;
  if (req.method !== 'GET') {
    event.respondWith(fetch(req));
    return;
  }

  const url = new URL(req.url);
  if (url.origin !== self.location.origin) {
    event.respondWith(fetch(req));
    return;
  }

  const accept = req.headers.get('accept') || '';
  if (req.mode === 'navigate' || accept.includes('text/html')) {
    event.respondWith(fetch(req));
    return;
  }

  event.respondWith(
    fetch(req)
      .then((res) => {
        if (res.ok) {
          const copy = res.clone();
          caches.open(CACHE_NAME).then((cache) => cache.put(req, copy));
        }
        return res;
      })
      .catch(() => caches.match(req))
  );
});
