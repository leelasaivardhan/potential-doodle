document.addEventListener('DOMContentLoaded',()=>{
  const state=document.getElementById('state'); const district=document.getElementById('district');
  if(state && district){ state.addEventListener('change', async()=>{ district.innerHTML='<option value="">Select District</option>'; if(!state.value)return; const r=await fetch('/Application/Districts?stateId='+state.value); const data=await r.json(); data.forEach(x=>{const o=document.createElement('option');o.value=x.districtId;o.textContent=x.name;district.appendChild(o);}); }); }
});
