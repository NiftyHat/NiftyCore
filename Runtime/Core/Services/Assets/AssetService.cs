
using NiftyFramework.Core.Services;

namespace NiftyFramework.Services
{
    [NiftyService]
    public abstract class AssetService<TAssetIndex> : NiftyService
    {
        //public const string PATH_INDEX = "AssetService/AssetIndex.asset";
        protected TAssetIndex _assetIndex;
        public TAssetIndex Index => _assetIndex;
        
        public abstract TAsset Get<TAsset>();
    }
}