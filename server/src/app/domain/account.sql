/*
Navicat MySQL Data Transfer

Source Server         : 阿里云
Source Server Version : 50722
Source Host           : 47.105.51.196:3306
Source Database       : passwordSave

Target Server Type    : MYSQL
Target Server Version : 50722
File Encoding         : 65001

Date: 2019-02-16 11:34:30
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for account
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(15) CHARACTER SET utf8 DEFAULT NULL,
  `password` varchar(40) CHARACTER SET utf8 DEFAULT NULL,
  `regtime` datetime DEFAULT NULL,
  `data` text CHARACTER SET utf8,
  `token` int(10) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=latin1;
