using System;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Services
{
    /// <summary>
    /// Singleton Service Locator để tránh tạo ra rác (Garbage Collection) khi khởi tạo Repository liên tục.
    /// Giúp quản lý bộ nhớ tốt hơn theo mô hình DI.
    /// </summary>
    public static class AppServices
    {
        private static Lazy<MovieRepository> _movieRepo = new Lazy<MovieRepository>(() => new MovieRepository());
        private static Lazy<AudioRepository> _audioRepo = new Lazy<AudioRepository>(() => new AudioRepository());
        private static Lazy<TagRepository> _tagRepo = new Lazy<TagRepository>(() => new TagRepository());
        private static Lazy<MovieService> _movieService = new Lazy<MovieService>(() => new MovieService());
        private static Lazy<BackupService> _backupService = new Lazy<BackupService>(() => new BackupService());
        private static Lazy<ActorRepository> _actorRepo = new Lazy<ActorRepository>(() => new ActorRepository());

        public static MovieRepository MovieRepo => _movieRepo.Value;
        public static AudioRepository AudioRepo => _audioRepo.Value;
        public static TagRepository TagRepo => _tagRepo.Value;
        public static MovieService MovieSvc => _movieService.Value;
        public static BackupService BackupSvc => _backupService.Value;
        public static ActorRepository ActorRepo => _actorRepo.Value;
    }
}
