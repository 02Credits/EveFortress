using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class NonCachingContentManager : ContentManager
    {
        public NonCachingContentManager(IServiceProvider services)
            : base(services) { }

        public override T Load<T>(string assetName)
        {
            return ReadAsset<T>(assetName, null);
        }
    }
}
