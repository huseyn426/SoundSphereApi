// ── SoundSphere Player ──
(function () {
    'use strict';

    const state = {
        queue: [],
        currentIndex: -1,
        isPlaying: false,
        audio: new Audio()
    };

    // ── DOM refs ──
    const bar = document.querySelector('.player-bar');
    if (!bar) return;

    const titleEl = bar.querySelector('.player-title');
    const subtitleEl = bar.querySelector('.player-subtitle');
    const coverEl = bar.querySelector('.player-cover');
    
    // Updated button refs
    const playBtn = document.getElementById('btn-play') || bar.querySelector('.player-main-btn');
    const prevBtn = document.getElementById('btn-prev');
    const nextBtn = document.getElementById('btn-next');
    const shuffleBtn = document.getElementById('btn-shuffle');
    const repeatBtn = document.getElementById('btn-repeat');
    const muteBtn = document.getElementById('btn-mute');
    const volumeSlider = document.getElementById('volume-slider');
    
    const progressFill = bar.querySelector('.progress-fill');
    const progressLine = bar.querySelector('.progress-line');
    const timeStart = bar.querySelectorAll('.player-progress span')[0];
    const timeEnd = bar.querySelectorAll('.player-progress span')[1];

    // State extensions
    state.isShuffle = false;
    state.repeatMode = 0; // 0: none, 1: all, 2: one
    state.originalQueue = [];
    state.isMuted = false;
    state.lastVolume = 1;

    // ── Format time ──
    function fmt(sec) {
        if (!sec || isNaN(sec)) return '0:00';
        const m = Math.floor(sec / 60);
        const s = Math.floor(sec % 60);
        return m + ':' + (s < 10 ? '0' : '') + s;
    }

    const queueBtn = document.getElementById('btn-queue');
    const queuePanel = document.getElementById('queue-panel');
    const queueList = document.getElementById('queue-list');
    const playerLikeBtn = document.getElementById('player-btn-like');

    // ── Load track ──
    function loadTrack(index) {
        if (index < 0 || index >= state.queue.length) return;
        state.currentIndex = index;
        const t = state.queue[index];

        titleEl.textContent = t.title || t.Title || 'Unknown';
        subtitleEl.textContent = t.artist || t.ArtistName || 'SoundSphere Player';

        const cover = t.cover || t.CoverImageUrl;
        if (cover) {
            coverEl.style.backgroundImage = 'url(' + cover + ')';
            coverEl.style.backgroundSize = 'cover';
        } else {
            coverEl.style.backgroundImage = '';
        }

        const url = t.audio || t.AudioUrl;
        if (url) {
            state.audio.src = url;
            state.audio.load();
        }

        timeEnd.textContent = t.duration || t.Duration || '0:00';
        progressFill.style.width = '0%';
        timeStart.textContent = '0:00';

        highlightCurrent();
        renderQueue();
        
        // Check if liked (simplified)
        if (playerLikeBtn) playerLikeBtn.innerHTML = '🤍';

        // Log to listening history
        const tid = t.id || t.Id || t.TrackId;
        if (tid) {
            fetch('/Music/LogPlay', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(parseInt(tid))
            }).catch(() => {});
        }
    }

    function renderQueue() {
        if (!queueList) return;
        queueList.innerHTML = '';
        state.queue.forEach((t, i) => {
            const item = document.createElement('div');
            item.className = 'queue-item' + (i === state.currentIndex ? ' active' : '');
            item.onclick = () => { loadTrack(i); togglePlay(); };
            
            const cover = t.cover || t.CoverImageUrl || '';
            item.innerHTML = `
                <div class="queue-img" style="background-image:url('${cover}')"></div>
                <div class="queue-info">
                    <div class="queue-title">${t.title || t.Title}</div>
                    <div class="queue-artist">${t.artist || t.ArtistName}</div>
                </div>
            `;
            queueList.appendChild(item);
        });
    }

    if (queueBtn) {
        queueBtn.onclick = () => queuePanel.classList.toggle('active');
    }

    if (playerLikeBtn) {
        playerLikeBtn.onclick = () => {
            const t = state.queue[state.currentIndex];
            const tid = t?.id || t?.Id || t?.TrackId;
            if (!tid) return;

            fetch('/Music/ToggleLike?trackId=' + tid, { method: 'POST' })
            .then(() => {
                playerLikeBtn.classList.toggle('active');
                playerLikeBtn.innerHTML = playerLikeBtn.classList.contains('active') ? '❤️' : '🤍';
            });
        };
    }


    // ── Play / Pause ──
    function togglePlay() {
        if (state.currentIndex < 0 && state.queue.length > 0) {
            loadTrack(0);
        }

        const url = state.queue[state.currentIndex]?.audio || state.queue[state.currentIndex]?.AudioUrl;

        if (url) {
            if (state.isPlaying) {
                state.audio.pause();
            } else {
                state.audio.play().catch(() => { });
            }
        }

        state.isPlaying = !state.isPlaying;
        if (playBtn) playBtn.textContent = state.isPlaying ? '⏸' : '▶';
    }

    function playNext() {
        if (state.currentIndex < state.queue.length - 1) {
            loadTrack(state.currentIndex + 1);
        } else if (state.repeatMode === 1 && state.queue.length > 0) { // Repeat All
            loadTrack(0);
        } else {
            return; // end of queue
        }
        
        if (state.queue[state.currentIndex].audio || state.queue[state.currentIndex].AudioUrl) {
            state.audio.play().catch(() => { });
            state.isPlaying = true;
            if (playBtn) playBtn.textContent = '⏸';
        }
    }

    function playPrev() {
        if (state.audio.currentTime > 3) {
            state.audio.currentTime = 0;
            return;
        }

        if (state.currentIndex > 0) {
            loadTrack(state.currentIndex - 1);
        } else if (state.repeatMode === 1 && state.queue.length > 0) {
            loadTrack(state.queue.length - 1);
        } else {
            state.audio.currentTime = 0;
        }

        if (state.queue[state.currentIndex]?.audio || state.queue[state.currentIndex]?.AudioUrl) {
            state.audio.play().catch(() => { });
            state.isPlaying = true;
            if (playBtn) playBtn.textContent = '⏸';
        }
    }

    // ── Progress ──
    state.audio.addEventListener('timeupdate', function () {
        if (state.audio.duration) {
            const pct = (state.audio.currentTime / state.audio.duration) * 100;
            progressFill.style.width = pct + '%';
            timeStart.textContent = fmt(state.audio.currentTime);
        }
    });

    state.audio.addEventListener('loadedmetadata', function () {
        timeEnd.textContent = fmt(state.audio.duration);
    });

    state.audio.addEventListener('ended', function () {
        if (state.repeatMode === 2) { // Repeat One
            state.audio.currentTime = 0;
            state.audio.play().catch(() => {});
        } else {
            playNext();
        }
    });

    // ── Click on progress bar ──
    if (progressLine) {
        progressLine.addEventListener('click', function (e) {
            if (state.audio.duration) {
                const rect = progressLine.getBoundingClientRect();
                const pct = (e.clientX - rect.left) / rect.width;
                state.audio.currentTime = pct * state.audio.duration;
            }
        });
    }

    // ── Highlight current track in list ──
    function highlightCurrent() {
        document.querySelectorAll('.track-item').forEach(function (el, i) {
            const isMatch = state.queue[state.currentIndex] && 
                            (el.dataset.title === state.queue[state.currentIndex].title || el.dataset.title === state.queue[state.currentIndex].Title);
            el.classList.toggle('track-playing', isMatch);
        });
    }

    // ── Button events ──
    if (playBtn) playBtn.addEventListener('click', togglePlay);
    if (prevBtn) prevBtn.addEventListener('click', playPrev);
    if (nextBtn) nextBtn.addEventListener('click', playNext);
    
    // Shuffle logic
    if (shuffleBtn) {
        shuffleBtn.addEventListener('click', function() {
            state.isShuffle = !state.isShuffle;
            shuffleBtn.classList.toggle('active', state.isShuffle);
            
            if (state.isShuffle) {
                state.originalQueue = [...state.queue];
                const currentTrack = state.queue[state.currentIndex];
                // Fisher-Yates shuffle
                for (let i = state.queue.length - 1; i > 0; i--) {
                    const j = Math.floor(Math.random() * (i + 1));
                    [state.queue[i], state.queue[j]] = [state.queue[j], state.queue[i]];
                }
                // Bring current to front
                if (currentTrack) {
                    const idx = state.queue.indexOf(currentTrack);
                    if (idx > 0) {
                        state.queue.splice(idx, 1);
                        state.queue.unshift(currentTrack);
                    }
                    state.currentIndex = 0;
                }
            } else {
                const currentTrack = state.queue[state.currentIndex];
                state.queue = [...state.originalQueue];
                if (currentTrack) {
                    state.currentIndex = state.queue.findIndex(t => t === currentTrack || t.id === currentTrack.id);
                }
            }
        });
    }

    // Repeat logic
    if (repeatBtn) {
        repeatBtn.addEventListener('click', function() {
            state.repeatMode = (state.repeatMode + 1) % 3;
            if (state.repeatMode === 0) {
                repeatBtn.classList.remove('active');
                repeatBtn.innerHTML = '🔁';
            } else if (state.repeatMode === 1) {
                repeatBtn.classList.add('active');
                repeatBtn.innerHTML = '🔁';
            } else {
                repeatBtn.classList.add('active');
                repeatBtn.innerHTML = '🔂'; // Repeat one
            }
        });
    }

    // Volume logic
    if (volumeSlider) {
        volumeSlider.addEventListener('input', function(e) {
            const vol = parseFloat(e.target.value);
            state.audio.volume = vol;
            if (vol === 0) {
                state.isMuted = true;
                if (muteBtn) muteBtn.innerHTML = '🔇';
            } else {
                state.isMuted = false;
                state.lastVolume = vol;
                if (muteBtn) muteBtn.innerHTML = '🔊';
            }
        });
    }

    if (muteBtn) {
        muteBtn.addEventListener('click', function() {
            state.isMuted = !state.isMuted;
            if (state.isMuted) {
                state.audio.volume = 0;
                if (volumeSlider) volumeSlider.value = 0;
                muteBtn.innerHTML = '🔇';
            } else {
                state.audio.volume = state.lastVolume || 1;
                if (volumeSlider) volumeSlider.value = state.lastVolume || 1;
                muteBtn.innerHTML = '🔊';
            }
        });
    }

    // ── Global API ──
    window.playerSetQueue = function (tracks, startIndex) {
        state.originalQueue = [...tracks];
        state.queue = tracks;
        
        if (state.isShuffle && tracks.length > 0) {
             for (let i = state.queue.length - 1; i > 0; i--) {
                const j = Math.floor(Math.random() * (i + 1));
                [state.queue[i], state.queue[j]] = [state.queue[j], state.queue[i]];
            }
            // we ignore startIndex if shuffled, just start at 0
            startIndex = 0;
        }
        
        loadTrack(startIndex || 0);
        state.isPlaying = false;
        togglePlay();
    };

    window.playTrackFromList = function (btn) {
        const item = btn.closest('.track-item');
        const items = Array.from(document.querySelectorAll('.track-item'));
        const idx = items.indexOf(item);

        const tracks = items.map(function (el) {
            return {
                id: el.dataset.trackId,
                title: el.dataset.title,
                artist: el.dataset.artist,
                audio: el.dataset.audio,
                cover: el.dataset.cover,
                duration: el.dataset.duration
            };
        });

        window.playerSetQueue(tracks, idx);
    };

    window.playerAppendToQueue = function (track) {
        if (!track || !track.audio) return;
        state.queue.push({
            id: track.id,
            title: track.title,
            artist: track.artist,
            audio: track.audio,
            cover: track.cover,
            duration: track.duration
        });
        renderQueue();
    };

    window.playPlaylist = function () {
        const btn = document.querySelector('.play-all-btn');
        if (!btn) return;
        try {
            const tracks = JSON.parse(btn.dataset.tracks);
            const mapped = tracks.map(function (t) {
                return {
                    title: t.Title,
                    artist: t.ArtistName,
                    audio: t.AudioUrl,
                    cover: t.CoverImageUrl,
                    duration: t.Duration
                };
            });
            window.playerSetQueue(mapped, 0);
        } catch (e) {
            console.error('playPlaylist error', e);
        }
    };

    window.playSingleTrack = function (title, artist, audioUrl, coverUrl, duration, id) {
        window.playerSetQueue([{
            id: id,
            title: title,
            artist: artist,
            audio: audioUrl,
            cover: coverUrl,
            duration: duration
        }], 0);
    };

    // ── Restore from sessionStorage ──
    try {
        const saved = sessionStorage.getItem('ssPlayer');
        if (saved) {
            const d = JSON.parse(saved);
            if (d.title) {
                titleEl.textContent = d.title;
                subtitleEl.textContent = d.artist || 'SoundSphere Player';
            }
        }
    } catch (e) { }

    // ── Keyboard Shortcuts ──
    document.addEventListener('keydown', function(e) {
        // Don't trigger shortcuts if user is typing in an input
        if (['INPUT', 'TEXTAREA'].includes(document.activeElement.tagName)) return;

        switch(e.code) {
            case 'Space':
                e.preventDefault();
                togglePlay();
                break;
            case 'ArrowRight':
                playNext();
                break;
            case 'ArrowLeft':
                playPrev();
                break;
        }
    });

    // ── Save state on unload ──
    window.addEventListener('beforeunload', function () {
        if (state.currentIndex >= 0 && state.queue[state.currentIndex]) {
            const t = state.queue[state.currentIndex];
            sessionStorage.setItem('ssPlayer', JSON.stringify({
                title: t.title || t.Title,
                artist: t.artist || t.ArtistName
            }));
        }
    });
})();
