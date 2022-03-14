using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalRankingModel  {
	public string user_id;
	public string user_name;
	public int totalStar;
	public int totalCrown;
	public string avatar;
}

public class GlobalRankingUser
{
	public List<GlobalRankingModel> data; 
	public int index_me;

	public GlobalRankingUser(){
		data = new List<GlobalRankingModel> ();
	}
}
