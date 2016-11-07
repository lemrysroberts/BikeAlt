///////////////////////////////////////////////////////////
// 
// GameTime.cs
//
// What it does: Keeps track of an independent time to that being used by Unity.
//				 Handy for pausing gameplay without bollocking up the game globally.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

// Enables the GameTime timer functions
#define GAMETIME_TIMING		

using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GameTime
{
	public event System.Action TimePaused;
	public event System.Action TimeUnpaused;
	
	public static GameTime Instance 
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new GameTime();
			}
			
			return s_instance;
		}
	}
	
	public static float deltaTime
	{
		get { return Instance.m_deltaTime; }
	}

    public static float time
    {
        get {  return Instance.m_time; }
    }

    public static float timeMultiplier
    {
        get { return Instance.m_timeMultiplier; }
    }

	public void Update()
	{
		if(!Paused)
		{
			m_deltaTime = Time.deltaTime * m_timeMultiplier;
            m_time      += Time.deltaTime * m_timeMultiplier;
		}
		else
		{
			m_deltaTime = 0.0f;	
		}
	}
	
	public bool Paused 
	{ 
		get
		{
			return m_paused;	
		}
		
		set
		{
			bool paused = value;
			
			if(paused != m_paused)
			{
				m_paused = paused; 
				
				if(m_paused && TimePaused != null)
				{
                    m_timeMultiplier = 0.0f;
                    TimePaused();	
				}
				else if(!m_paused && TimeUnpaused != null)
				{
                    m_timeMultiplier = 1.0f;
                    TimeUnpaused();
				}
			}
		}
	}



	public void StartTimer()
	{
#if GAMETIME_TIMING
		m_stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif
	}

	public long StopTimer()
	{
#if GAMETIME_TIMING
		return m_stopwatch.ElapsedMilliseconds;
#else
		return 0;
#endif
	}
	
	private GameTime()
	{
#if GAMETIME_TIMING
		m_stopwatch = new System.Diagnostics.Stopwatch();
#endif
	}
	
    [SerializeField]
    public float m_time                                 = 0.0f;

    [SerializeField]
    public float m_deltaTime							= 0.0f;

    [SerializeField]
    public float m_timeMultiplier                       = 1.0f;
    private static GameTime s_instance 					= null;

    [SerializeField]
    private bool m_paused 								= false;

#if GAMETIME_TIMING
	private System.Diagnostics.Stopwatch m_stopwatch	= null;	
#endif
}
