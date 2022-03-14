#!/usr/bin/python

import sys, os, syslog
from mod_pbxproj import XcodeProject
from mod_pbxproj import PBXBuildFile


syslog.openlog( 'MoPub' )
syslog.syslog( syslog.LOG_ALERT, '--------------- excecuting MoPub post processor ------------------' )

pathToProjectFile = sys.argv[1] + '/Unity-iPhone.xcodeproj/project.pbxproj'
pathToMoPubFolder = sys.argv[2]
project = XcodeProject.Load( pathToProjectFile )


project.add_file_if_doesnt_exist( 'System/Library/Frameworks/AdSupport.framework', tree='SDKROOT', weak=True )
project.add_file_if_doesnt_exist('System/Library/Frameworks/CoreData.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/Security.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist('System/Library/Frameworks/StoreKit.framework', tree='SDKROOT')
project.add_file_if_doesnt_exist( 'usr/lib/libsqlite3.0.dylib', tree='SDKROOT' )
project.add_file_if_doesnt_exist( 'usr/lib/libz.1.2.5.dylib', tree='SDKROOT' )
project.add_file_if_doesnt_exist( 'usr/lib/libxml2.2.dylib', tree='SDKROOT' )
project.add_other_ldflags('-ObjC')



if project.modified:
    syslog.syslog( syslog.LOG_ALERT, 'saving Xcode project file modifications' )
    project.save()
