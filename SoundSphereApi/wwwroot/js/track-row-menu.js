(function () {
    'use strict';

    function rowFromTrigger(btn) {
        return btn.closest('.track-item');
    }

    function trackPayload(row) {
        if (!row || !row.dataset) return null;
        return {
            id: row.dataset.trackId,
            title: row.dataset.title,
            artist: row.dataset.artist,
            audio: row.dataset.audio,
            cover: row.dataset.cover || '',
            duration: row.dataset.duration || '0:00'
        };
    }

    function closeAllMenus() {
        document.querySelectorAll('.track-menu-dropdown').forEach(function (d) {
            d.hidden = true;
        });
        document.querySelectorAll('.track-menu-trigger').forEach(function (t) {
            t.setAttribute('aria-expanded', 'false');
        });
    }

    document.addEventListener('click', function (e) {
        var trigger = e.target.closest('.track-menu-trigger');
        if (trigger) {
            e.preventDefault();
            e.stopPropagation();
            var wrap = trigger.closest('.track-menu-wrap');
            var menu = wrap && wrap.querySelector('.track-menu-dropdown');
            if (!menu) return;
            var open = !menu.hidden;
            closeAllMenus();
            if (!open) {
                menu.hidden = false;
                trigger.setAttribute('aria-expanded', 'true');
            }
            return;
        }

        if (e.target.closest('.track-menu-dropdown')) return;
        closeAllMenus();
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeAllMenus();
    });

    document.addEventListener('click', function (e) {
        var item = e.target.closest('.track-menu-item');
        if (!item) return;
        var row = item.closest('.track-item');
        if (!row) return;
        var action = item.getAttribute('data-action');
        var t = trackPayload(row);
        if (!t || !t.audio) return;

        closeAllMenus();

        if (action === 'play') {
            if (typeof window.playerSetQueue === 'function') {
                window.playerSetQueue(
                    [
                        {
                            id: t.id,
                            title: t.title,
                            artist: t.artist,
                            audio: t.audio,
                            cover: t.cover,
                            duration: t.duration
                        }
                    ],
                    0
                );
            }
            return;
        }

        if (action === 'playlist') {
            if (typeof window.openAddToPlaylistModal === 'function') {
                window.openAddToPlaylistModal(parseInt(t.id, 10));
            }
            return;
        }

        if (action === 'like') {
            if (t.id) {
                fetch('/Music/ToggleLike?trackId=' + encodeURIComponent(t.id), { method: 'POST' }).catch(function () {});
            }
            return;
        }

        if (action === 'queue') {
            if (typeof window.playerAppendToQueue === 'function') {
                window.playerAppendToQueue({
                    id: t.id,
                    title: t.title,
                    artist: t.artist,
                    audio: t.audio,
                    cover: t.cover,
                    duration: t.duration
                });
            }
        }
    });
})();
