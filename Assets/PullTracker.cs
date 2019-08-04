using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullTracker : MonoBehaviour
{
    private ParasiteHead p_head;
    private SpriteRenderer sr;
    private GameObject tracker;

    private void Awake () {
        p_head = GameObject.FindWithTag("ParasiteHead").GetComponent<ParasiteHead>();
        sr = GetComponent<SpriteRenderer>();
        tracker = GameObject.Find("MouseTrackerReverse").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(p_head.IsGrabbed()) {
            if(!sr.enabled)
                sr.enabled = true;
            if(!tracker.activeSelf)
                tracker.SetActive(true);
            Vector3 p_pos = p_head.transform.position;
            Vector3 m_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_pos.z = 0;
            Vector3 p_m_diff = p_pos - m_pos;
            Vector3 m_pos_reverse = p_pos + p_m_diff;
            Vector3 p_mr_diff = m_pos_reverse - p_pos;
            Vector3 mid_point = Vector3.Lerp(p_pos, m_pos_reverse, 0.5f);
            transform.position = mid_point;
            transform.localScale = new Vector3(transform.localScale.x, p_mr_diff.magnitude, 1);
            float angle = Vector3.SignedAngle(Vector3.up, p_mr_diff, Vector3.forward);
            transform.localEulerAngles = new Vector3(0,0,angle);
            tracker.transform.position = m_pos_reverse;
        }
        else {
            if (sr.enabled)
                sr.enabled = false;
            if(tracker.activeSelf)
                tracker.SetActive(false);
        }
    }
}
