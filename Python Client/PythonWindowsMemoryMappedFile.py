import win32event
import win32api
import winerror
import mmap
import os
import sys
import numpy
import pygame
import struct
import time
import string

height = 480
width = 640
start_time = time.time()

try:
    for i in range(5000):
        print "in for loop"
        mutex = win32event.CreateMutex(None, 0, 'kinectColorMutex')
        print "created mutex"
        win32event.WaitForSingleObject(mutex, win32event.INFINITE)
        print "got mutex"
        start_time = time.time()
        print "prior to creating mmap"
        mm = mmap.mmap(0, 1228800, 'kinectColorData')
        print "prior to seek"
        mm.seek(0)
        binaryData = numpy.fromstring(mm.read(1228800), dtype="B", count=-1, sep="")
        print "read mmf"
        mm.close()
        print "closed memory map"
        win32event.ReleaseMutex(mutex)
        print "released mutex"
        colorHold = binaryData[0:1228800].reshape( (height, width, 4) )
        colorData = numpy.ones( (height, width, 3), dtype='B' )
        colorData[:,:,2] = colorHold[:,:,0]
        colorData[:,:,1] = colorHold[:,:,1]
        colorData[:,:,0] = colorHold[:,:,2]
        print "processed array"

        pygameSurface = pygame.surfarray.make_surface(numpy.transpose(colorData, (1,0,2)))
        pygame.init()
        screen = pygame.display.set_mode((width, height),0,24)
        screen.blit(pygameSurface, (0,0))
        pygame.display.update()

        win32api.CloseHandle(mutex)

except AttributeError, a:
    print a
    print ''
except NameError, n:
    print n
    print ''
except TypeError, e:
    print e
    print ''
except:
    print sys.exc_info() [0]

pygame.quit()
print "end"
