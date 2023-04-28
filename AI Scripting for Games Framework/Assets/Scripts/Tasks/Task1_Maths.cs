using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task1_Maths : MonoBehaviour
{
    [SerializeField]
    Text m_DotProduct;
    [SerializeField]
    Text m_Degrees;
    [SerializeField]
    Text m_Radians;
    [SerializeField]
    Transform m_VectorArrow;
    [SerializeField]
    Transform m_UnitCircle;

    [SerializeField]
    Transform m_MovingBox;
    [SerializeField]
    Transform m_StaticBox;
    [SerializeField]
    Text m_Distance;
    [SerializeField]
    Text m_Magnitude;
    [SerializeField]
    Text m_Normalised;
    [SerializeField]
    Text m_StaticPosition;
    [SerializeField]
    Text m_MovingPosition;


    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            //Calculate a vector from centre of circle to the mouse position.
            Vector3 toMousePos = Input.mousePosition - m_UnitCircle.position;
            Debug.Log(toMousePos);

            //Normalize it.
            Vector2 unitVectorToMousePos = Maths.Normalise(toMousePos);

            //Then find the dot product from the up vector (-1,1).
            float dot = Maths.Dot(Vector2.up, unitVectorToMousePos);
            if (m_DotProduct != null)
            {
                m_DotProduct.text = dot.ToString("#0.000");
            }

            //Get the Radians.
            float radians = Mathf.Acos(dot);
            if (m_Radians != null)
            {
                m_Radians.text = radians.ToString("#0.000");
            }

            //Get the degrees.
            float degrees = radians * Mathf.Rad2Deg;
            if (m_Degrees != null)
            {
                m_Degrees.text = degrees.ToString("#0.000");
            }

            //-----------------------------------------------------------------------------------------
            //We need to know whether this is a right or left rotation.
            float dotRight = Maths.Dot(Vector2.right, unitVectorToMousePos);

            //Default to a left rotation.
            int dir = 1;

            //If the dot product is greater than 0.0, then it indicates a rotation to the right.
            if (dotRight > 0.0f)
            {
                dir = -1;
            }
            RotateArrow(degrees, dir);
        }

        if (Input.GetMouseButton(0))
        {
            m_MovingBox.position = Input.mousePosition;
        }
            Vector2 distance = m_MovingBox.position - m_StaticBox.position;
            float mag = Maths.Magnitude(distance);
            Vector2 norm = Maths.Normalise(distance);

            if (m_Distance != null)
            {
                m_Distance.text = distance.ToString();
            }

            if (m_Magnitude != null)
            {
                m_Magnitude.text = mag.ToString("#0.000");
            }

            if (m_Normalised != null)
            {
                m_Normalised.text = norm.ToString();
            }

        m_MovingPosition.text = m_MovingBox.position.ToString();
        m_StaticPosition.text = m_StaticBox.position.ToString();
    }

    void RotateArrow(float degrees, int dir)
    {
        //Rotate the arrow to face this direction.
        Vector3 euler = new Vector3(0.0f, 0.0f, degrees * dir);

        Quaternion newRotation = Quaternion.identity;
        newRotation.eulerAngles = euler;
        m_VectorArrow.transform.rotation = newRotation;
    }
}
