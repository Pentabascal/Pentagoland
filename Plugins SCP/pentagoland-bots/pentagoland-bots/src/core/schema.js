'use strict';
// Roles de staff compartidos por los tres bots (se detectan por el nombre que crea pentagoland-setup.js).
module.exports.staffRoleSchema = [
  { key: 'role_admin', type: 'role', label: 'Rol: Administración', detect: 'Administración' },
  { key: 'role_head', type: 'role', label: 'Rol: Head Staff', detect: 'Head Staff' },
  { key: 'role_mod', type: 'role', label: 'Rol: Moderator', detect: 'Moderator' },
  { key: 'role_trial', type: 'role', label: 'Rol: Trial Moderator', detect: 'Trial Moderator' },
  { key: 'role_support', type: 'role', label: 'Rol: Support', detect: 'Support' },
  { key: 'role_dev', type: 'role', label: 'Rol: Developer', detect: 'Developer' },
];
