import { HttpInterceptorFn } from '@angular/common/http';

export const addTokenInterceptor: HttpInterceptorFn = (req, next) => {
  let headers = req.headers
    .set('Content-Type', 'application/json')
    .set('Access-Control-Allow-Headers', 'Content-Type');

  const token = getToken();

  if (token !== null) headers = headers.set('Authorization', `Bearer ${token}`);

  const clonedRequest = req.clone({ headers });

  return next(clonedRequest);
};

function getToken(): string | null {
  return localStorage.getItem('token');
}
