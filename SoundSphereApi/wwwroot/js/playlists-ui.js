(function () {
    'use strict';

    function readPlaylists() {
        var el = document.getElementById('ss-user-playlists-json');
        if (!el || !el.textContent) return [];
        try {
            return JSON.parse(el.textContent);
        } catch (e) {
            return [];
        }
    }

    function ensureToastContainer() {
        var c = document.getElementById('ss-toast-container');
        if (!c) {
            c = document.createElement('div');
            c.id = 'ss-toast-container';
            c.className = 'ss-toast-container';
            document.body.appendChild(c);
        }
        return c;
    }

    function showToast(message) {
        var c = ensureToastContainer();
        var t = document.createElement('div');
        t.className = 'ss-toast';
        t.textContent = message;
        c.appendChild(t);
        requestAnimationFrame(function () {
            t.classList.add('ss-toast-visible');
        });
        setTimeout(function () {
            t.classList.remove('ss-toast-visible');
            setTimeout(function () {
                t.remove();
            }, 320);
        }, 3200);
    }

    function closeModal() {
        var m = document.getElementById('ss-playlist-modal');
        if (m) {
            m.classList.remove('ss-modal-open');
            setTimeout(function () {
                m.remove();
            }, 280);
        }
    }

    function addTrackToPlaylist(trackId, playlistId, playlistName) {
        var token = localStorage.getItem('token');
        if (!token) {
            window.location.href = '/Account/Login';
            return;
        }

        fetch('/api/Playlists/add-track', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: 'Bearer ' + token
            },
            body: JSON.stringify({ playlistId: playlistId, trackId: trackId })
        })
            .then(function (r) {
                return r.json().then(function (data) {
                    return { ok: r.ok, status: r.status, data: data };
                });
            })
            .then(function (res) {
                var msg = (res.data && res.data.message) || '';
                if (res.ok) {
                    closeModal();
                    var label = playlistName || 'playlist';
                    showToast('Added to ' + label + ' ✓');
                    return;
                }
                showToast(msg || 'Could not add track');
            })
            .catch(function () {
                showToast('Network error');
            });
    }

    function openAddToPlaylistModal(trackId) {
        var existing = document.getElementById('ss-playlist-modal');
        if (existing) existing.remove();

        var playlists = readPlaylists();
        var overlay = document.createElement('div');
        overlay.id = 'ss-playlist-modal';
        overlay.className = 'ss-modal-overlay';
        overlay.setAttribute('role', 'dialog');
        overlay.setAttribute('aria-modal', 'true');

        var panel = document.createElement('div');
        panel.className = 'ss-modal-panel ss-modal-panel--playlists';
        panel.addEventListener('click', function (e) {
            e.stopPropagation();
        });

        var head = document.createElement('div');
        head.className = 'ss-modal-header';
        head.innerHTML =
            '<h2 class="ss-modal-title">Add to playlist</h2>' +
            '<button type="button" class="ss-modal-close" aria-label="Close">&times;</button>';
        head.querySelector('.ss-modal-close').addEventListener('click', closeModal);

        var body = document.createElement('div');
        body.className = 'ss-modal-body';

        if (!playlists.length) {
            var empty = document.createElement('div');
            empty.className = 'ss-modal-empty';
            empty.innerHTML =
                '<p>No playlists yet. Create one first.</p>' +
                '<a href="/Playlists/Create" class="btn-primary ss-modal-create-btn">Create playlist</a>';
            body.appendChild(empty);
        } else {
            var list = document.createElement('ul');
            list.className = 'ss-playlist-pick-list';
            playlists.forEach(function (p) {
                var li = document.createElement('li');
                var btn = document.createElement('button');
                btn.type = 'button';
                btn.className = 'ss-playlist-pick-item';
                var cover = document.createElement('div');
                cover.className = 'ss-playlist-pick-cover';
                if (p.coverImageUrl) {
                    cover.style.backgroundImage = 'url(' + p.coverImageUrl + ')';
                } else {
                    cover.classList.add('ss-playlist-pick-cover--empty');
                    cover.textContent = '♪';
                }
                var txt = document.createElement('div');
                txt.className = 'ss-playlist-pick-text';
                var nm = document.createElement('span');
                nm.className = 'ss-playlist-pick-name';
                nm.textContent = p.name;
                txt.appendChild(nm);
                btn.appendChild(cover);
                btn.appendChild(txt);
                btn.addEventListener('click', function () {
                    addTrackToPlaylist(trackId, p.id, p.name);
                });
                li.appendChild(btn);
                list.appendChild(li);
            });
            body.appendChild(list);
        }

        panel.appendChild(head);
        panel.appendChild(body);
        overlay.appendChild(panel);

        overlay.addEventListener('click', closeModal);

        document.body.appendChild(overlay);
        requestAnimationFrame(function () {
            overlay.classList.add('ss-modal-open');
        });
    }

    window.openAddToPlaylistModal = openAddToPlaylistModal;
    window.showPlaylistToast = showToast;
})();
