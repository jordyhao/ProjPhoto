﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelJournal.PCL.DataService;

namespace TravelJournal.WP8.Test
{
    public class MockPhotoManager:IPhotoManager
    {
        private List<Photo> photos;

        public void LoadMockData(List<Photo> mockPhoto)
        {
            photos = mockPhoto;
        }

        public void ProceedRawPhoto(DateTime tag, Action<Photo> onPhotoFoundHandler)
        {
            foreach (Photo photo in photos.Where((p) => { return p.Position.GpsPoint.Timestamp > tag; }))
            {
                if (onPhotoFoundHandler != null)
                    onPhotoFoundHandler(photo);
            }
        }

        public bool CheckRawPhoto(DateTime tag)
        {
            return photos.Any((p) => { return p.Position.GpsPoint.Timestamp > tag; });
        }
    }
}
