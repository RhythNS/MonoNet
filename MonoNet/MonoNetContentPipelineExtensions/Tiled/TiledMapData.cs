namespace MonoNetContentPipelineExtensions
{
    public class TiledMapData
    {
        public string Data { get; private set; }
        public string FileName { get; private set; }

        public TiledMapData(string data, string filename = null)
        {
            Data = data;
            FileName = filename;
        }
    }
}
