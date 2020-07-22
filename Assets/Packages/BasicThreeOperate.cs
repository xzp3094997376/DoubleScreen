using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicThreeOperate : GrabbableBase<Draggable.Grabber>
//, IInitializePotentialDragHandler
//, IBeginDragHandler
//, IDragHandler
//, IEndDragHandler
{
    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    var hitDistance = 0f;

    //    switch (eventData.button)
    //    {
    //        case PointerEventData.InputButton.Middle:
    //        case PointerEventData.InputButton.Right:
    //            hitDistance = Mathf.Min(eventData.pointerPressRaycast.distance, m_initGrabDistance);
    //            break;
    //        case PointerEventData.InputButton.Left:
    //            hitDistance = eventData.pointerPressRaycast.distance;
    //            break;
    //        default:
    //            return;
    //    }

    //    var grabber = Grabber.Get(eventData);
    //    grabber.grabber2hit = new RigidPose(new Vector3(0f, 0f, hitDistance), Quaternion.identity);
    //    grabber.hit2pivot = RigidPose.FromToPose(grabber.grabberOrigin * grabber.grabber2hit, new RigidPose(transform));

    //    if (m_eventGrabberSet == null) { m_eventGrabberSet = new IndexedTable<PointerEventData, Grabber>(); }
    //    m_eventGrabberSet.Add(eventData, grabber);

    //    AddGrabber(grabber);
    //}

    //public void OnDrag(PointerEventData eventData)
    //{

    //}
    //protected virtual void Update()
    //{
    //    if (!isGrabbed) { return; }

    //    if (!moveByVelocity)
    //    {
    //        RecordLatestPosesForDrop(Time.time, 0.05f);
    //        OnGrabTransform();
    //    }

    //    var scrollDelta = currentGrabber.eventData.scrollDelta * m_scrollingSpeed;
    //    if (scrollDelta != Vector2.zero)
    //    {
    //        currentGrabber.hitDistance = Mathf.Max(0f, currentGrabber.hitDistance + scrollDelta.y);
    //    }
    //}
    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    //if (m_eventGrabberSet == null) { return; }

    //    //Grabber grabber;
    //    //if (!m_eventGrabberSet.TryGetValue(eventData, out grabber)) { return; }

    //    RemoveGrabber(grabber);
    //    m_eventGrabberSet.Remove(eventData);
    //    Grabber.Release(grabber);
    //}

    //public void OnInitializePotentialDrag(PointerEventData eventData)
    //{
    //    eventData.useDragThreshold = false;
    //}
    public Pointer3DRaycaster raycaster;
    public void Awake()
    {
        //var grabber = Grabber.Get(eventData);
    }
}
