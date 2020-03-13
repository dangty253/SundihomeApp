using System;
using SundihomeApp.IServices;
using SundihomeApp.Services;

namespace SundihomeApp.iOS.Services
{
    public class MediaPickerService : IMediaPickerService
    {
        public IMultiMediaPickerService GetMultiMediaPickerService()
        {
            return new MultiMediaPickerService();
        }
    }
}
