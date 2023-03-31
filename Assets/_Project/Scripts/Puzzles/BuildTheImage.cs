using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTheImage : MonoBehaviour
{
    public Animator m_Animator;
    public int unblockedPines = 0;

    public bool internalMoved = true;
    public GameObject emptyInternalAxis;
    public GameObject internalAxis;
    public int internalDiscRotation = 0;

    public bool externalMoved = true;
    public GameObject emptyExternalAxis;
    public GameObject externalAxis;
    public int ExternalDiscRotation = 0;

    public bool lateralMoved = true;
    public GameObject emptyLateralAxis;
    public GameObject lateralAxis;
    public int lateralDiscRotation = 0;

    public GameObject lateralAxisSeccion1;
    public GameObject lateralAxisSeccion2;
    public GameObject lateralAxisSeccion3;

    public GameObject lateralSeccion1;
    public GameObject lateralSeccion2;
    public GameObject lateralSeccion3;
    public GameObject lateralSeccion4;
    public GameObject lateralSeccion5;
    public GameObject lateralSeccion6;
    public GameObject lateralSeccion7;
    public GameObject lateralSeccion8;

    public bool isAligned = true;

    // aver se me está ocurriendo utilizar animaciones para que se encarguen de hacer todo el trabajo de calcular rotaciones y mmdas

    public void takeInternal()
    {
        if(internalMoved)
        {
            internalAxis.transform.SetParent(emptyInternalAxis.transform);
            internalMoved = !internalMoved;
            VerifyUnblockedPines();
        }
        else
        {
            internalAxis.transform.SetParent(this.transform);
            internalMoved = !internalMoved;
            VerifyUnblockedPines();
        }
    }

    public void TakeExternal() // debería poner como argumento que entreguen tres hijos a los cuales rotar
    {
        if (lateralAxisSeccion1 != null & lateralAxisSeccion2 != null & lateralAxisSeccion3 != null)
        {
            if (externalMoved)
            {
                lateralAxisSeccion1.transform.SetParent(emptyExternalAxis.transform);
                lateralAxisSeccion2.transform.SetParent(emptyExternalAxis.transform);
                lateralAxisSeccion3.transform.SetParent(emptyExternalAxis.transform);
                externalAxis.transform.SetParent(emptyExternalAxis.transform);
                externalMoved = !externalMoved;
                isAligned = !isAligned;
                m_Animator.SetBool("isAligned", isAligned);
                VerifyUnblockedPines();
            }
            else
            {
                lateralAxisSeccion1.transform.SetParent(lateralAxis.transform);
                lateralAxisSeccion2.transform.SetParent(lateralAxis.transform);
                lateralAxisSeccion3.transform.SetParent(lateralAxis.transform);
                externalAxis.transform.SetParent(this.transform);
                externalMoved = !externalMoved;
                VerifyUnblockedPines();
            }
        } 
    }

    public void takeLateral() // cada que se llame este... va a cambiar los tres secciones que se les dará como hijos al eje exterior
    {
        if (isAligned == true)
        {
            if(lateralMoved)
            {
                lateralDiscRotation++;

                lateralAxis.transform.SetParent(emptyLateralAxis.transform);
                lateralMoved = !lateralMoved;
                VerifyUnblockedPines();

                if (lateralDiscRotation > 3)
                    lateralDiscRotation = 0;


                if (lateralDiscRotation == 0)
                {
                    lateralAxisSeccion1 = lateralSeccion1;
                    lateralAxisSeccion2 = lateralSeccion2;
                    lateralAxisSeccion3 = lateralSeccion3;
                }
                if (lateralDiscRotation == 1)
                {
                    lateralAxisSeccion1 = lateralSeccion3;
                    lateralAxisSeccion2 = lateralSeccion4;
                    lateralAxisSeccion3 = lateralSeccion5;
                }
                if (lateralDiscRotation == 2)
                {
                    lateralAxisSeccion1 = lateralSeccion5;
                    lateralAxisSeccion2 = lateralSeccion6;
                    lateralAxisSeccion3 = lateralSeccion7;

                }
                if (lateralDiscRotation == 3)
                {
                    lateralAxisSeccion1 = lateralSeccion7;
                    lateralAxisSeccion2 = lateralSeccion8;
                    lateralAxisSeccion3 = lateralSeccion1;
                }
            }
            else
            {


                lateralAxis.transform.SetParent(this.transform);
                lateralMoved = !lateralMoved;
                VerifyUnblockedPines();
            }
        }
    }

    public void VerifyUnblockedPines()
    {
        if(unblockedPines == 5)
        {
            print("se ha desbloqueado el cerebro");
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Verificador"))
        {
            unblockedPines--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Verificador"))
        {
            unblockedPines++;
            VerifyUnblockedPines();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
