using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SoundSphereApi.Helpers
{
    public static class UiText
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Data = new()
        {
            ["en"] = new Dictionary<string, string>
            {
                ["brand"] = "SoundSphere",
                ["home"] = "Home",
                ["tracks"] = "Tracks",
                ["login"] = "Login",
                ["register"] = "Register",
                ["logout"] = "Logout",
                ["search_placeholder"] = "Search tracks, artists, albums...",
                ["hero_badge"] = "Music platform",
                ["hero_title"] = "Feel every track in one clean space.",
                ["hero_desc"] = "Discover music, build playlists, follow activity, and manage your listening experience through a dark, modern interface inspired by streaming platforms.",
                ["hero_button_primary"] = "Open Tracks",
                ["hero_button_secondary"] = "Create Account",
                ["panel_title"] = "Your personal sound hub",
                ["panel_desc"] = "Search faster, listen smarter, and keep your music flow organized in one place.",
                ["feature_1_title"] = "Fast Discovery",
                ["feature_1_desc"] = "Search across tracks, artists, albums, and genres through a cleaner music-first interface.",
                ["feature_2_title"] = "Playlist Flow",
                ["feature_2_desc"] = "Build and manage playlists with the backend already connected and ready.",
                ["feature_3_title"] = "User Space",
                ["feature_3_desc"] = "Authentication, history, subscriptions, likes, and comments are already part of the platform.",
                ["tracks_title"] = "Tracks",
                ["tracks_empty"] = "No tracks found.",
                ["search_button"] = "Search",
                ["clear_button"] = "Clear",
                ["artist"] = "Artist",
                ["album"] = "Album",
                ["genre"] = "Genre",
                ["plays"] = "plays",
                ["duration"] = "Duration",
                
                // New Keys
                ["profile"] = "Profile",
                ["subscriptions"] = "Subscriptions",
                ["dashboard"] = "Dashboard",
                ["admin"] = "Admin",
                ["play_track"] = "Play Track",
                ["like"] = "Like",
                ["comments"] = "Comments",
                ["create_playlist"] = "Create Playlist",
                ["my_playlists"] = "My Playlists",
                ["recently_played"] = "Recently Played",
                ["edit_profile"] = "Edit Profile"
            },
            ["ru"] = new Dictionary<string, string>
            {
                ["brand"] = "SoundSphere",
                ["home"] = "Главная",
                ["tracks"] = "Треки",
                ["login"] = "Вход",
                ["register"] = "Регистрация",
                ["logout"] = "Выход",
                ["search_placeholder"] = "Поиск по трекам, артистам, альбомам...",
                ["hero_badge"] = "Музыкальная платформа",
                ["hero_title"] = "Почувствуй каждый трек в одном стильном пространстве.",
                ["hero_desc"] = "Открывай музыку, собирай плейлисты, следи за активностью и управляй своим музыкальным опытом через современный темный интерфейс в духе стриминговых сервисов.",
                ["hero_button_primary"] = "Открыть треки",
                ["hero_button_secondary"] = "Создать аккаунт",
                ["panel_title"] = "Твой личный музыкальный центр",
                ["panel_desc"] = "Быстрый поиск, удобное прослушивание и аккуратная организация музыки в одном месте.",
                ["feature_1_title"] = "Быстрый поиск",
                ["feature_1_desc"] = "Ищи треки, артистов, альбомы и жанры через интерфейс, заточенный под музыку.",
                ["feature_2_title"] = "Плейлисты",
                ["feature_2_desc"] = "Создавай и управляй плейлистами, пока backend уже полностью готов.",
                ["feature_3_title"] = "Профиль",
                ["feature_3_desc"] = "Авторизация, история, подписки, лайки и комментарии уже встроены в платформу.",
                ["tracks_title"] = "Треки",
                ["tracks_empty"] = "Треки не найдены.",
                ["search_button"] = "Поиск",
                ["clear_button"] = "Сброс",
                ["artist"] = "Артист",
                ["album"] = "Альбом",
                ["genre"] = "Жанр",
                ["plays"] = "прослушиваний",
                ["duration"] = "Длительность",

                // New Keys
                ["profile"] = "Профиль",
                ["subscriptions"] = "Подписки",
                ["dashboard"] = "Панель управления",
                ["admin"] = "Админ",
                ["play_track"] = "Слушать",
                ["like"] = "Нравится",
                ["comments"] = "Комментарии",
                ["create_playlist"] = "Создать плейлист",
                ["my_playlists"] = "Мои плейлисты",
                ["recently_played"] = "Недавно прослушанные",
                ["edit_profile"] = "Редактировать профиль"
            }
        };

        public static string Get(HttpContext context, string key)
        {
            var lang = context.Session.GetString("lang") ?? "en";

            if (!Data.ContainsKey(lang))
            {
                lang = "en";
            }

            if (Data[lang].TryGetValue(key, out var value))
            {
                return value;
            }

            return key;
        }

        public static string Current(HttpContext context)
        {
            return context.Session.GetString("lang") ?? "en";
        }
    }
}
