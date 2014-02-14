﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using TravelJournal.WinForm.Simulator.Forms;

namespace TravelJournal.WinForm.Simulator.Controls
{
    public partial class TravelMapPlayer : UserControl,ITestControl
    {
        public static string ID_ANCHORS_LAYER = "ID_ANCHORS_LAYER";
        public static string ID_HOMEPLACEMARKS_LAYER = "ID_HOMEPLACEMARKS_LAYER";
        public static string ID_ANCHORS_ROUTE_LAYER = "ID_ANCHORS_ROUTE_LAYER";
        public static string ID_ANCHORS_ROUTE = "ID_ANCHORS_ROUTE";

        private const int RATIO_MAX_ZOOM = 18;
        private const int RATIO_MIN_ZOOM = 1;
        private const int RATIO_DEFAULT_ZOOM = 2;
        private const int RATIO_DEFAULT_ZOOM_COUNTRY = 10;
        private const int AMOUNT_PHOTO_YELLOW_MARKER = 5;
        private const int AMOUNT_PHOTO_ORANGE_MARKER = 10;

        private GMapOverlay anchorsLayer = new GMapOverlay(ID_ANCHORS_LAYER);
        private GMapOverlay homePlacemarkLayer = new GMapOverlay(ID_HOMEPLACEMARKS_LAYER);
        private GMapOverlay anchorsRouteLayer = new GMapOverlay(ID_ANCHORS_ROUTE_LAYER);

        public GMapControl GMapControl { get { return gMapControl; } }

        public TravelMapPlayer()
        {
            InitializeComponent();
            // Load layers
            gMapControl.Overlays.Add(anchorsLayer);
            gMapControl.Overlays.Add(homePlacemarkLayer);
            gMapControl.Overlays.Add(anchorsRouteLayer);
        }

        private void TravelMapPlayer_Load(object sender, EventArgs e)
        {
            InitializeGMap();
        }

        private void InitializeGMap()
        {
            Provider = GMap.NET.MapProviders.GMapProviders.BingMap;
            gMapControl.DragButton = MouseButtons.Left;
            GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gMapControl.MaxZoom = RATIO_MAX_ZOOM;
            gMapControl.MinZoom = RATIO_MIN_ZOOM;
            gMapControl.Zoom = RATIO_DEFAULT_ZOOM;
        }

        public GMapProvider Provider
        {
            get { return gMapControl.MapProvider; }
            set { gMapControl.MapProvider = value; }
        }

        public void FocusOn(string keywords, int zoom = RATIO_DEFAULT_ZOOM)
        {
            gMapControl.SetPositionByKeywords(keywords);
            gMapControl.Zoom = zoom;
        }

        public int GetAnchorIndex(GMapMarker item)
        {
            return anchorsLayer.Markers.IndexOf(item);
        }
        public void SetAnchors(List<SimulationModelPoint> anchors)
        {
            anchorsLayer.Markers.Clear();
            foreach (SimulationModelPoint anchor in anchors)
            {
                GMarkerGoogleType markerType;
                if (anchor.PhotoGenNumber == 0)
                    markerType = GMarkerGoogleType.blue_small;
                else if (anchor.PhotoGenNumber < AMOUNT_PHOTO_YELLOW_MARKER)
                    markerType = GMarkerGoogleType.yellow_small;
                else if (anchor.PhotoGenNumber < AMOUNT_PHOTO_ORANGE_MARKER)
                    markerType = GMarkerGoogleType.orange_small;
                else
                    markerType = GMarkerGoogleType.red_small;
                anchorsLayer.Markers.Add(new GMarkerGoogle(anchor.Gps, markerType));
            }
        }
        public void SetHomePlace(Placemark placeMark)
        {
            if (placeMark.CountryName != string.Empty)
            {
                // Get keyword
                GeoCoderStatusCode code;
                string keyword = placeMark.LocalityName ?? (placeMark.DistrictName ?? placeMark.AdministrativeAreaName);
                if (keyword != string.Empty)
                    keyword += ",";
                keyword += placeMark.CountryName;
                // Get gps out of keyword
                PointLatLng point = GMapProviders.GoogleMap.GetPoint(keyword, out code).Value;
                // Initialize position if no anchors are added
                if (anchorsLayer.Markers.Count == 0)
                {
                    gMapControl.Position = point;
                    gMapControl.Zoom = RATIO_DEFAULT_ZOOM_COUNTRY;
                }
                // Place marker
                GMarkerGoogle marker = new GMarkerGoogle(point, GMarkerGoogleType.red_pushpin);
                homePlacemarkLayer.Markers.Clear();
                homePlacemarkLayer.Markers.Add(marker);
            }
        }
        public void ConnectAnchors()
        {
            GMapRoute routes = new GMapRoute(ID_ANCHORS_ROUTE);
            routes.Stroke.Width = 2;
            routes.Stroke.Color = Color.DodgerBlue;
            for(int i=0;i<anchorsLayer.Markers.Count;i++)
                routes.Points.Add(anchorsLayer.Markers[i].Position);
            routes.Points.Add(anchorsLayer.Markers[0].Position);
            anchorsRouteLayer.Clear();
            anchorsRouteLayer.Routes.Add(routes);
            // Clear the anchors
            anchorsLayer.Markers.Clear();
        }
        public void DisconnectAnchors()
        {
            anchorsRouteLayer.Clear();
        }

        public SimulationModelPoint ConvertToSimulationModelPoint(Point localPoint)
        {
            PointLatLng gps = gMapControl.FromLocalToLatLng(localPoint.X, localPoint.Y);
            return new SimulationModelPoint() { Gps = gps };
        }
        public Placemark ConvertToPlacemark(Point localPoint)
        {
            PointLatLng gps = gMapControl.FromLocalToLatLng(localPoint.X, localPoint.Y);
            // Get geo coordinate
            List<Placemark> plc = null;
            var st = GMapProviders.GoogleMap.GetPlacemarks(gps, out plc);
            return plc[0];
        }

        private void test()
        {
            GMapOverlay markersOverlay = new GMapOverlay("markers");
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(49 + 6 / 60.0 + 11.35 / 3600.0, 6 + 13 / 60.0 + 12.558 / 3600.0),
              GMarkerGoogleType.red_big_stop);
            markersOverlay.Markers.Add(marker);
            gMapControl.Overlays.Add(markersOverlay);

            List<Placemark> plc = null;
            var st = GMapProviders.GoogleMap.GetPlacemarks(new PointLatLng(49 + 6 / 60.0 + 11.35 / 3600.0, 6 + 13 / 60.0 + 12.558 / 3600.0), out plc);
            if (st == GeoCoderStatusCode.G_GEO_SUCCESS && plc != null)
            {
                foreach (var pl in plc)
                {
                    if (!string.IsNullOrEmpty(pl.PostalCodeNumber))
                    {
                        
                    }
                }
            }
        }

        public void Initialize()
        {
            InitializeGMap();
        }


    }
}
