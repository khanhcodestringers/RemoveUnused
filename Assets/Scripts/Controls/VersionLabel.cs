using UnityEngine;
using System.Collections;

namespace Mio {
    public class VersionLabel : MonoBehaviour {
        public UILabel lbVersion;

        void Start() {
            lbVersion.text = "v"+ TrackedBundleVersion.Current.GetVersionString();
        }
    }
}