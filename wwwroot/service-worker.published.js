self.importScripts('./service-worker-assets.js');

self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheName = 'blazor-resources';
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/, /\.svg$/ ];
const offlineAssetsExclude = []; //[ /^service-worker\.js$/ ];

async function onInstall(event) {
    try {
        var request = await fetch("https://firestore.googleapis.com/v1/projects/web-paragon/databases/(default)/documents/Version/Version");
    } catch (e) {
        console.log("Offline");
        return;
    }
    
    var requestText = await request.text();

    var latestVersion = JSON.parse(requestText).fields.Version.integerValue;

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        //.map(asset => new Request(asset.url, { integrity: asset.hash }));
        .map(asset => new Request(asset.url));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));

    await (await caches.open("Version")).put("Version", new Response(latestVersion));
}

async function onActivate(event) {
    var request = await fetch("https://firestore.googleapis.com/v1/projects/web-paragon/databases/(default)/documents/Version/Version")
    var requestText = await request.text();
    var latestVersion = JSON.parse(requestText).fields.Version.integerValue;

    var versionCache = await caches.open("Version");
    var storedVersion = await versionCache.match("Version");

    if (latestVersion != storedVersion) {
        onInstall();
    }

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate';

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        const cache = await caches.open(cacheName);

        let result = await cache.match(request);
        if (result instanceof Response) return result;
        else return fetch(request);
    }

    return fetch(event.request);
}