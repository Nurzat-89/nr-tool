# KazNRDC tool to calculate nuclide number densities for Massive Stars
This application is designed to calculate the nuclear number density of isotopes in massive stars as a function of time. It is built to support advanced research in nuclear astrophysics, particularly in modeling stellar nucleosynthesis. The application relies on ENDF (Evaluated Nuclear Data File) datasets for nuclear reaction data.

## Features
* **Nuclear Number Density Calculation:** Calculates isotope concentrations over time using input nuclear data.
* **High-Accuracy Methods:** Implements robust numerical methods like CRAM (Chebyshev Rational Approximation Method) and MMPA for matrix exponentials.
* **Customizable Input:** Supports user-provided ENDF files for tailored calculations

## Prerequisites
1. System Requirements
   * **Operating System:** Windows
   * **.NET Runtime:** .NET 6.0 or later
   * **RAM:** Minimum 4GB (higher recommended for large datasets)
2. Dependencies
   * **ENDF (Evaluated Nuclear Data Files) datasets.** You can obtain these from [IAEA ENDF Database](https://www-nds.iaea.org/exfor/endf.htm).

# Installation
* **Download the Data Library.** First download and install Evaluated Nuclear Data File (ENDF) installer:
  * [ENDFB-VIII](https://drive.google.com/file/d/13xvVk2kN6klo8WLAxsGl8bLR67xJtqiF/view?usp=sharing)
* **Download the Tool.** Download the latest version of the KazNRDC tool:
  *  [1.0.6](https://drive.google.com/file/d/1uJD0mueM_WGa90zDZfMG45u1vXzJgVTi/view?usp=sharing)
  *  [1.0.5](https://drive.google.com/file/d/1Z0W1F7b07-5T1ufLCoOc3Wirwj1-ez4u/view?usp=sharing)
* **Default Data Library.** The application currently includes only one default data library: ENDFB-VIII. The associated data files are located in:
  > ```%LocalAppData%/KazNRDC/xsdir```
* **Additional Data Libraries.**
  * If you require additional data libraries, please contact: Nurzat Kenzhebaev 📧 nurzat.kenzhebaev@gmail.com
  * Currently only these data libraries are available:
    * EndfB-VIII (by default)
    * Jeff
    * Jendl
    * Tendl
