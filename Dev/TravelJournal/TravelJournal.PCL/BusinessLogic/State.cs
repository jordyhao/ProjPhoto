﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelJournal.PCL.DataService;

namespace TravelJournal.PCL.BusinessLogic
{
    public abstract class State
    {
        public abstract void Execute(Processor processor);
     
        public virtual void ChangeState(Processor p, State s)
        {
            p.State = s;
        }
    }

    public class OriginalState : State
    {
        public override void Execute(Processor processor)
        {
  

        }
    }
    public class PilotState : State
    {
        public override void Execute(Processor processor)
        {

            processor.TourRoutePoints.Add(processor.UserPosition.GpsPoint);      
        }

    }
    public class PhotoHandlerState : State
    {
        public override void Execute(Processor processor)
        {

            processor.TourRoutePoints.Add(processor.UserPosition.GpsPoint);
            processor.Album = processor.DataManager.GetAlbum("test");
            processor.PhotoManager.FoundRawPhoto(processor.Album.TimeTag, processor.PhotoHandler);

        }
    }
    public class AlbumGeneratorState : State
    {
        public override async void Execute(Processor processor)
        {
            foreach (Photo p in processor.Album.PhotoList)
            {
                if (processor.TouristCity.Contains(p.Position.City) == false)
                    processor.TouristCity.Add(p.Position.City);
            }
            foreach (GpsPoint p in processor.TourRoutePoints)
            {
               GpsPosition TourRoutePosition =  await processor.WebService.GetGeoposition(p);
               if (processor.TouristCity.Contains(TourRoutePosition.City) == false)
                   processor.TourRoutePoints.Remove(p);
            }
        }
    }

}
