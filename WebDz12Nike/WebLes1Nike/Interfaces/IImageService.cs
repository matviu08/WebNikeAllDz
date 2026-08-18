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
        /// <returns>Ім'я збереженого файлу (без шляху)</returns>
        Task<string> SaveOptimizedImageAsync(IFormFile file);
        Task<string> SaveOptimizedImageAsync(string base64Image);
        Task RemoveImageAsync(string imageName);

    }
}

