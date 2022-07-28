using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archangel
{
    public class SwordsController : MonoBehaviour
    {
        private PrintController leftSwordShowController;
        private PrintController leftSwordHideController;

        private PrintController rightSwordShowController;
        private PrintController rightSwordHideController;

        public GameObject leftSword;
        public GameObject rightSword;

        private void Start()
        {
            var leftSwordPrintControllers = leftSword.GetComponents<SwordPrintController>();
            leftSwordShowController = leftSwordPrintControllers.FirstOrDefault(c => c.controllerName == "Show");
            leftSwordHideController = leftSwordPrintControllers.FirstOrDefault(c => c.controllerName == "Hide");

            var rightSwordPrintControllers = rightSword.GetComponents<SwordPrintController>();
            rightSwordShowController = rightSwordPrintControllers.FirstOrDefault(c => c.controllerName == "Show");
            rightSwordHideController = rightSwordPrintControllers.FirstOrDefault(c => c.controllerName == "Hide");
        }

        public void ShowLeft()
        {
            leftSword.SetActive(true);
            leftSwordShowController.enabled = true;
            leftSwordHideController.enabled = false;
        }

        public void ShowRight()
        {
            rightSword.SetActive(true);
            rightSwordShowController.enabled = true;
            rightSwordHideController.enabled = false;
        }

        public void HideLeft()
        {
            leftSwordHideController.enabled = true;
            leftSwordShowController.enabled = false;
        }

        public void HideRight()
        {
            rightSwordHideController.enabled = true;
            rightSwordShowController.enabled = false;
        }
    }
}
