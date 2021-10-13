============================================================
XR Assembly
============================================================


Visualize Assembly data in XR


Main features
-------------

* feature
* feature
* more features

**xr_assembly** runs on Python x.x and x.x.


Documentation
-------------

.. Explain how to access documentation: API, examples, etc.

..
.. optional sections:

Requirements
------------

.. Write requirements instructions here


Dependencies
------------

* `Assembly Information Model <https://github.com/augmentedfabricationlab/assembly_information_model>`_


Installation of Dependencies
----------------------------
**Assembly Information Model**
::
    (your_env_name) python -m pip install git+https://github.com/augmentedfabricationlab/assembly_information_model@master#egg=assembly_information_model
    (your_env_name) python -m compas_rhino.install -p assembly_information_model

**XR Assembly**
::
    (your_env_name) python -m pip install git+https://github.com/augmentedfabricationlab/xr_assembly@master#egg=xr_assembly
    (your_env_name) python -m compas_rhino.install -p xr_assembly -v 7.0


Contributing
------------

Make sure you setup your local development environment correctly:

* Clone the `xr_assembly <https://github.com/augmentedfabricationlab/xr_assembly>`_ repository.
* Install development dependencies and make the project accessible from Rhino:

::

    (your_env_name) pip install -r requirements-dev.txt
    (your_env_name) invoke add-to-rhino


Credits
-------------

This package was created by Lidia Atanasova <lidia.atanasova@tum.de> `@lidiatanasova <https://github.com/lidiatanasova>`_ at `@augmentedfabricationlab <https://github.com/augmentedfabricationlab>`_
