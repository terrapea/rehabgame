using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// JobManager is just a proxy object so we have a launcher for the coroutines
/// </summary>
public class JobManager : IxoMonoSingleton<JobManager> {
}

public class Job
{
	public event System.Action<bool> jobComplete;
	
	private bool _running;
	public bool Running { get { return _running; } }
	
	private bool _paused;
	public bool Paused { get { return _paused; } }
	
	private bool _jobWasKilled;
	public bool JobKilled { get { return _jobWasKilled; } }
	
	public bool Completed { get { return !_running && !_paused && _coroutine == null; } }
	
	private IEnumerator _coroutine;
	private Stack<Job> _childJobStack;
	
	#region constructors
	public Job (IEnumerator coroutine) : this ( coroutine, true)
	{}
	
	public Job (IEnumerator coroutine, bool shouldStart)
	{
		_coroutine = coroutine;
		if( shouldStart )
			Start();
	}
	
	#endregion
	
	#region static Job makers
	public static Job Make(IEnumerator coroutine)
	{
		return new Job (coroutine);
	}
	
	public static Job Make(IEnumerator coroutine, bool shouldStart)
	{
		return new Job(coroutine, shouldStart);
	}
	#endregion
	
	#region public API
	public Job CreateAndAddChildJob(IEnumerator coroutine)
	{
		Job j = new Job(coroutine, false);
		AddChildJob(j);
		return j;
	}
	
	public void AddChildJob(Job childJob)
	{
		if(_childJobStack == null)
			_childJobStack = new Stack<Job>();
		_childJobStack.Push(childJob);
	}
	
	public void removeChildJob(Job childJob)
	{
		if(_childJobStack.Contains(childJob))
		{
			Stack<Job> childStack = new Stack<Job>(_childJobStack.Count-1);
			Job [] allCurrentChildren = _childJobStack.ToArray();
			System.Array.Reverse(allCurrentChildren);
			
			for(int i = 0; i < allCurrentChildren.Length; i++)
			{
				Job j = allCurrentChildren[i];
				if(j != childJob)
					childStack.Push(j);
			}
			
			//assign the new stack
			_childJobStack = childStack;
		}
	}
	
	public void Start()
	{
		_running = true;
		JobManager.Instance.StartCoroutine(DoWork());
	}
	
	public IEnumerator StartAsCoroutine()
	{
		_running = true;
		yield return JobManager.Instance.StartCoroutine(DoWork());
	}
	
	public void Pause()
	{
		_paused = true;
	}
	
	public void Unpause()
	{
		_paused = false;
	}
	
	public void Kill()
	{
		_jobWasKilled = true;
		_running = false;
		_paused = false;
	}
	
	public void Kill(float delayInSeconds)
	{
		int delay = (int)(delayInSeconds * 1000);
		new System.Threading.Timer( (obj) => 
		{
			lock(this)
			{
				Kill();
			}
		}, null, delay, System.Threading.Timeout.Infinite);
	}
	
	#endregion
	
	private IEnumerator DoWork()
	{
		//null out the first run through in case we start paused
		yield return null;
		
		while(_running)
		{
			if(_paused)
				yield return null;
			else
			{
				//run one iteration and we are done
				if(_coroutine.MoveNext())
					yield return _coroutine.Current;
				else
				{
					if(_childJobStack != null)
						yield return JobManager.Instance.StartCoroutine(RunChildJobs());
					_running = false;
				}
			}
		}
		
		if(jobComplete != null)
			jobComplete(_jobWasKilled);
		
		_coroutine = null;
		_childJobStack = null;
	}
	
	private IEnumerator RunChildJobs()
	{
		if(_childJobStack != null && _childJobStack.Count > 0)
		{
			do
			{
				Job childJob = _childJobStack.Pop();
				yield return JobManager.Instance.StartCoroutine( childJob.StartAsCoroutine());
			}
			while (_childJobStack.Count > 0);
		}
	}
}