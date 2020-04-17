using UnityEngine;
using System.Collections;

[RequireComponent(typeof (BuddyBotMove))]
public class BuddyBotBrain : MonoBehaviour
{

    private BuddyBotMove m_Mover;
    private Stabiliser m_Stabiliser;

    public enum States
    {
        Idle,
        Scanning,
        FollowingRoute,
        StoppingAtEndOfRoute,
        AdjustHeight
    }

    public States State;
    public LayerMask ObstacleLayers;
    public float PathPointTolerance = 1f;
    public float ScanPointTolerance = 0.2f;
    public float ScanPointRotationTolerance = 5f;
    public float StopSpeedToleranceSqr = 0.2f;
    public float HoverHeight = 2f;

    public Scanner Scanner;

    public ScanTarget CurrentTarget;

    public void Awake()
    {
        m_Mover = GetComponent<BuddyBotMove>();
        m_Stabiliser = GetComponent<Stabiliser>();
    }

    public void Start()
    {
        State = States.Idle;
    }

    private Vector3[] _currentPath;
    private int _currentPathIndex;

    public int NavMeshLayerMask = (1 << 0) | (1 << 2);

    public void GoTo(Vector3 position, bool useTargetHeight)
    {
        // Find a path along the floor to the target position
        RaycastHit myGroundInfo, targetGroundInfo;
        if (!Physics.Raycast(transform.position, Vector3.down, out myGroundInfo, 100f, ObstacleLayers))
        {
           // Debug.LogError("Buddybot couldn't find ground underneath itself.", this);
            return;
        }
            
        if(!Physics.Raycast(position, Vector3.down, out targetGroundInfo, 100f, ObstacleLayers))
        {
           // Debug.LogError("Buddybot couldn't find ground underneath the target position " + position);
            return;
        }

        var path = new UnityEngine.AI.NavMeshPath();
        if (!UnityEngine.AI.NavMesh.CalculatePath(myGroundInfo.point, targetGroundInfo.point, NavMeshLayerMask, path))
        {
            // Debug.LogWarning("Buddybot couldn't find a path from current ground point " + myGroundInfo.point + " to target ground point " + targetGroundInfo.point);
            return;
        }

        _currentPath = path.corners;
        _currentPathIndex = 0;

        // Lift the path up off the ground
        for (int i = 0; i < _currentPath.Length; ++i)
        {
            _currentPath[i].y += HoverHeight;
        }

        if (useTargetHeight)
            _currentPath[_currentPath.Length - 1] = position;

        m_Mover.Target = _currentPath[0];
        m_Stabiliser.SetTargetDirection(_currentPath[1] - _currentPath[0]);
        State = States.FollowingRoute;
    }

    public Transform Friend;
    public float MinFriendDistance = 2f;
    public float MaxFriendDistance = 4f;
	public float IdleHoverHeight = 3f;

    public float MaxFriendWaitTime = 3f;
    public float LastTargetArrivalTime;

    public void Update()
    {
        switch (State)
        {
            case States.Idle:
            {
                if(SelectNewTarget())
                    break;

                if (Vector3.Distance(Friend.position, transform.position) > MaxFriendDistance ||
                    (LastTargetArrivalTime < Time.time - MaxFriendWaitTime))
                {
                    Vector3 floorPosNearFriend;
                    if (FindRandomPositionNearFriend(out floorPosNearFriend))
                    {
                        GoTo(floorPosNearFriend + Vector3.up*IdleHoverHeight, true);
                    }
                }

                break;
            }
            case States.FollowingRoute:
            {
                var distanceToNextPoint = Vector3.Distance(transform.position, _currentPath[_currentPathIndex]);

                if (distanceToNextPoint < PathPointTolerance)
                {
                    ++_currentPathIndex;
                    if (_currentPathIndex >= _currentPath.Length)
                    {
                        State = States.StoppingAtEndOfRoute;
                    }
                    else
                    {
                        m_Mover.Target = _currentPath[_currentPathIndex];
                        if (_currentPathIndex < _currentPath.Length - 1)
                        {
                            m_Stabiliser.SetTargetDirection(_currentPath[_currentPathIndex] -
                                                            _currentPath[_currentPathIndex - 1]);
                        }
                    }
                }
                break;
            }

            case States.StoppingAtEndOfRoute:
            {
                if (GetComponent<Rigidbody>().velocity.sqrMagnitude <= StopSpeedToleranceSqr)
                {
                    LastTargetArrivalTime = Time.time;
                    if (CurrentTarget)
                    {
                        State = States.AdjustHeight;
                    }
                    else
                    {
                        State = States.Idle;
                    }
                }
                break;
            }

            case States.AdjustHeight:
            {
                m_Mover.Target = CurrentTarget.GetScanPosition();
                m_Stabiliser.SetTargetDirection(CurrentTarget.GetScanDirection());

                var distanceToPoint = Vector3.Distance(transform.position, m_Mover.Target);
                if (distanceToPoint < ScanPointTolerance && m_Stabiliser.GetAngleToTarget() < ScanPointRotationTolerance)
                {
                    StartCoroutine(PerformScan());
                }
                break;
            }

            case States.Scanning:
            {
                m_Stabiliser.SetTargetDirection((Scanner.TargetPoint - transform.position).normalized);
                break;
            }
        }
    }

    private bool FindRandomPositionNearFriend(out Vector3 result)
    {
        result = Vector3.zero;

        // No targets available - go and wait by the player
        UnityEngine.AI.NavMeshHit friendNavMeshInfo;
        UnityEngine.AI.NavMesh.SamplePosition(Friend.position, out friendNavMeshInfo, 5f, -1);

        for (int i = 0; i < 10; ++i)
        {
            var direction = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up) * Vector3.forward;
            result = friendNavMeshInfo.position + direction*Random.Range(MinFriendDistance, MaxFriendDistance);
            UnityEngine.AI.NavMeshHit candidatePointInfo;
            if (UnityEngine.AI.NavMesh.Raycast(friendNavMeshInfo.position, result, out candidatePointInfo, -1))
                result = candidatePointInfo.position;
            if (Vector3.Distance(result, friendNavMeshInfo.position) > MinFriendDistance)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator PerformScan()
    {
        State = States.Scanning;

        Scanner.beamAngleHorizontal = CurrentTarget.GetScanHorizontalMax();
        Scanner.beamAngleVertical = CurrentTarget.GetScanVerticalMax();
        Scanner.SweepHorizontally = CurrentTarget.GetShouldSweepHorizontally();

        Scanner.TurnOn(CurrentTarget.GetScanSurfacePoint(Scanner.transform.position, Scanner.BlockingLayers));

        yield return new WaitForSeconds(1.0f);

        Scanner.BeginScanning();

        float scanTime = CurrentTarget.GetScanTime();
        while (scanTime > 0 && CurrentTargetGroup)
        {
            yield return null;
            scanTime -= Time.deltaTime;
        }

        Scanner.EndScanning();
        Scanner.TurnOff();

        while (Scanner.BeamExtension > 0)
            yield return null;

        CurrentTarget.LastScannedTime = Time.time;
        CurrentTarget = null;

        if (CurrentTargetGroup != null)
        {
            yield return new WaitForSeconds(1.5f);
        }

        m_Stabiliser.LevelOff();
        State = States.Idle;
    }

    public ScanTargetGroup CurrentTargetGroup;

    private bool SelectNewTarget()
    {
        if (CurrentTargetGroup == null)
            return false;

        var newTarget = CurrentTargetGroup.SelectNewTarget();
        if (!newTarget) 
            return false;

        CurrentTarget = newTarget;
        GoTo(newTarget.GetScanPosition(), false);
        return true;
    }

    public void ClearCurrentTargetGroup()
    {
        CurrentTargetGroup = null;
    }

    // Work around bug 670290 by accepting a GameObject rather than a ScanTargetGroup
    public void SetCurrentTargetGroup(GameObject groupObj)
    {
        CurrentTargetGroup = groupObj.GetComponent<ScanTargetGroup>();
    }
}