using System.Collections.Generic;

namespace Mio.TileMaster {
    public class StoreDataModel {
        public int version;
        public List<SongDataModel> listAllSongs;
        public List<SongDataModel> listHotSongs;
        public List<SongDataModel> listNewSongs;


        public SongDataModel GetSongDataModelById(int id)
        {
            for (int i = 0; i < listAllSongs.Count; i++)
            {
                if (listAllSongs[i].ID == id)
                    return listAllSongs[i];
            }

            return null;
        }
    }
}