namespace WebLes1Nike.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Стискає та зберігає зображення на диск.
        /// Незалежно від початкового розміру/формату, файл буде перекодований
        /// у JPEG зі зменшеною шириною та заданою якістю стиснення.
        /// </summary>
        /// <param name="file">Файл, переданий користувачем (IFormFile)</param>
        /// <param name="folderPath">Абсолютний шлях до папки, куди зберігати (напр. wwwroot/images)</param>
        /// <returns>Ім'я збереженого файлу (без шляху)</returns>
        Task<string> SaveOptimizedImageAsync(IFormFile file, string folderPath);

        Task RemoveImageAsync(string imageName, string folderPath);

    }
}

