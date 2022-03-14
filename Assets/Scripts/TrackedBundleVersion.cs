using System.Collections;

// GENERATED CODE.
public class TrackedBundleVersion
{
	public static readonly string bundleIdentifier = "com.DefaultCompany.KPiano";

	public static readonly TrackedBundleVersionInfo Version_1_0_0 =  new TrackedBundleVersionInfo ("1.0", 0);
	public static readonly TrackedBundleVersionInfo Version_1_0_1_1 =  new TrackedBundleVersionInfo ("1.0.1", 1);
	public static readonly TrackedBundleVersionInfo Version_1_1_2 =  new TrackedBundleVersionInfo ("1.1", 2);
	public static readonly TrackedBundleVersionInfo Version_1_2_3 =  new TrackedBundleVersionInfo ("1.2", 3);
	public static readonly TrackedBundleVersionInfo Version_1_3_4 =  new TrackedBundleVersionInfo ("1.3", 4);
	public static readonly TrackedBundleVersionInfo Version_1_3_1_5 =  new TrackedBundleVersionInfo ("1.3.1", 5);
	public static readonly TrackedBundleVersionInfo Version_1_5_1_6 =  new TrackedBundleVersionInfo ("1.5.1", 6);
	public static readonly TrackedBundleVersionInfo Version_1_7 =  new TrackedBundleVersionInfo ("1", 7);
	public static readonly TrackedBundleVersionInfo Version_0_1_8 =  new TrackedBundleVersionInfo ("0.1", 8);
	
	public static readonly TrackedBundleVersion Instance = new TrackedBundleVersion ();

	public static TrackedBundleVersionInfo Current { get { return Instance.current; } }

	public static int CurrentBuildVersion { get { return 1; } }

	public ArrayList history = new ArrayList ();

	public TrackedBundleVersionInfo current = Version_0_1_8;

	public  TrackedBundleVersion() {
		history.Add (Version_1_0_0);
		history.Add (Version_1_0_1_1);
		history.Add (Version_1_1_2);
		history.Add (Version_1_2_3);
		history.Add (Version_1_3_4);
		history.Add (Version_1_3_1_5);
		history.Add (Version_1_5_1_6);
		history.Add (Version_1_7);
		history.Add (current);
	}

}
