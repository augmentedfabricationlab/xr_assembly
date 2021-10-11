from __future__ import absolute_import
from __future__ import division
from __future__ import print_function


from assembly_information_model.assembly import Element

import json


__all__ = ['Element']


class XRElement(Element):
    """Data structure representing a discrete element of an assembly for vizualization in XR.
    Attributes
    ----------
    frame : :class:`compas.geometry.Frame`
        The frame of the element.
    Examples
    --------
    >>> from compas.datastructures import Mesh
    >>> from compas.geometry import Box
    >>> element = Element.from_box(Box(Frame.worldXY(), ))
    """

    def __init__(self, frame):
        super(XRElement, self).__init__(frame)
        self.message = "Hello"

    def copy(self):
        """Returns a copy of this element.
        Returns
        -------
        Element
        """
        elem = XRElement(self.frame.copy())
        if self._tool_frame:
            elem._tool_frame = self._tool_frame.copy()
        if self.frame:
            elem.frame = self.frame.copy()
        if self._source:
            elem._source = self._source.copy()
        return elem


    def get_pose_quaternion(self):
        """ formats the element's frame to a pose quaternion and returns the pose"""
        return list(self.frame.point) + list(self.frame.quaternion)




