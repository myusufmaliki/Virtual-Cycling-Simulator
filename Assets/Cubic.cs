using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;




public class Cubic : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM7", 9600);
    void Start()
    {
        sp.Open();
        sp.ReadTimeout = 1;
    }

    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                if (sp.ReadByte() == 1)
                {
                    transform.Translate(Vector3.left * Time.deltaTime * 50);
                }
                if (sp.ReadByte() == 2)
                {
                    transform.Translate(Vector3.right * Time.deltaTime * 50);
                }

            }
            catch (System.Exception)
            {

            }

        }
    }
}
