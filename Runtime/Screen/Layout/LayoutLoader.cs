using JESUIS.Shared.ScreenData;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JESUIS.Runtime.Screen.Layout
{
    public class LayoutLoader
    {
        Dictionary<System.Guid, ScreenMetaData> screenMetaDataLookup = new Dictionary<System.Guid, ScreenMetaData>();

        public void BuildLayoutLookup()
        {
            foreach (var screenMetaData in Resources.LoadAll<ScreenMetaData>(""))
            {
                if (Guid.TryParse(screenMetaData.Uid, out Guid uid))
                {
                    if (screenMetaDataLookup.ContainsKey(uid))
                    {
                        throw new Exception($"screen layout look up contains duplicate key. {uid} {screenMetaData.name} {screenMetaDataLookup[uid].name}");
                    }

                    screenMetaDataLookup[uid] = screenMetaData;
                }
                else
                {
                    throw new Exception("ScreenMetaData has an invalid UID: " + screenMetaData.name);
                }
            }
        }

        public Shared.ScreenData.Screen LoadLayout(System.Guid uid)
        {
            if (screenMetaDataLookup.TryGetValue(uid, out ScreenMetaData screenMetaData))
            {
                string finalPath = Path.Combine(screenMetaData.Path, screenMetaData.FileName);
                return Resources.Load<JESUIS.Shared.ScreenData.Screen>(finalPath);
            }
            else
            {
                throw new System.Exception($"Failed to find target screen by uid {uid}");
            }
        }
    }
}

