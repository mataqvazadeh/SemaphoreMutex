// Ignore Spelling: Mutex

namespace SemaphoreMutex
{
    internal interface IIdGeneratorResource
    {
        int GetLatestId();
        void SetLatestId(int id);
    }
}
