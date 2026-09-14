const CACHE_NAME = "techlogistics-v2";

self.addEventListener("install", event => {
    self.skipWaiting();
});

self.addEventListener("activate", event => {
    event.waitUntil(
        caches.keys().then(cacheNames =>
            Promise.all(
                cacheNames
                    .filter(name => name !== CACHE_NAME)
                    .map(name => caches.delete(name))
            )
        )
    );

    self.clients.claim();
});

self.addEventListener("fetch", event => {
    const request = event.request;

    // No interceptar API, autenticación ni peticiones que no sean GET.
    if (
        request.method !== "GET" ||
        request.url.includes("/api/")
    ) {
        return;
    }

    event.respondWith(
        fetch(request)
            .then(response => {
                if (!response || response.status !== 200) {
                    return response;
                }

                const responseClone = response.clone();

                caches.open(CACHE_NAME)
                    .then(cache => {
                        cache.put(request, responseClone);
                    });

                return response;
            })
            .catch(() => {
                return caches.match(request);
            })
    );
});