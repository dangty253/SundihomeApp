using System;
using SundihomeApp.IServices;

namespace SundihomeApp.Droid.Services
{
    public class MediaPickerService : IMediaPickerService
    {
        public IMultiMediaPickerService GetMultiMediaPickerService()
        {
            return MultiMediaPickerService.SharedInstance;
        }
    }
}
