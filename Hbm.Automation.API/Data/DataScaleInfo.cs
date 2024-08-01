using System;

namespace Hbm.Automation.Api.Data
{
    internal class DataScaleInfo
    {
        #region =============== constructors & destructors =================

        public DataScaleInfo(INetConnection Connection)
        {
        }

        #endregion

        #region ==================== events & delegates ====================

        /// <summary>
        ///     Updates and converts the values from buffer
        /// </summary>
        /// <param name="sender">Connection class</param>
        /// <param name="e">EventArgs, Event argument</param>
        public void UpdateScalInfo(object sender, EventArgs e)
        {
        }

        #endregion

        #region ======================== properties ========================

        private int ScaleSupplyNominalVoltage { get; }
        private int ScaleSupplyMinimumVoltage { get; }
        private int ScaleSupplyMaximumVoltage { get; }
        private int ScaleAccuracyClass { get; set; }
        private int ScaleMinimumDeadLoad { get; }
        private int ScaleMaximumNumberVerificationInterval { get; }
        private int ScaleApportionmentFactor { get; }
        private int ScaleSafeLoadLimit { get; }
        private int ScaleOperationNominalTemperature { get; }
        private int ScaleOperationMinimumTemperature { get; }
        private int ScaleOperationMaximumTemperature { get; }
        private int ScaleRelativeMinimumLoadCellVerficationInterval { get; }
        private int ImplementedProfileSpecification { get; }
        private int LcCapability { get; }
        private int Alarms { get; }
        private string OimlCertificationInformation { get; }
        private string NtepCertificationInformation { get; }

        #endregion
    }
}